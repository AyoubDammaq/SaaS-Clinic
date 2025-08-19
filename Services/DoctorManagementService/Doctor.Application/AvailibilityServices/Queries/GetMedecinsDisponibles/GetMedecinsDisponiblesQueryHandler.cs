using Doctor.Application.DTOs;
using Doctor.Application.Interfaces;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.AvailibilityServices.Queries.GetMedecinsDisponibles
{
    public class GetMedecinsDisponiblesQueryHandler
        : IRequestHandler<GetMedecinsDisponiblesQuery, List<Medecin>>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        private readonly IRendezVousHttpClient _rdvClient;
        private readonly ILogger<GetMedecinsDisponiblesQueryHandler> _logger;

        private static readonly TimeSpan DureeCreneau = TimeSpan.FromMinutes(30);

        public GetMedecinsDisponiblesQueryHandler(
            IDisponibiliteRepository disponibiliteRepository,
            IRendezVousHttpClient rdvClient,
            ILogger<GetMedecinsDisponiblesQueryHandler> logger)
        {
            _disponibiliteRepository = disponibiliteRepository;
            _rdvClient = rdvClient;
            _logger = logger;
        }

        public async Task<List<Medecin>> Handle(GetMedecinsDisponiblesQuery request, CancellationToken cancellationToken)
        {
            if (request.date == default)
                throw new ArgumentException("La date ne peut pas être vide.", nameof(request.date));

            var day = request.date.DayOfWeek;
            var medecins = await _disponibiliteRepository.ObtenirMedecinsAvecDisponibilitesAsync();
            var medecinsDisponibles = new List<Medecin>();

            foreach (var medecin in medecins)
            {
                var dispos = medecin.Disponibilites.Where(d => d.Jour == day).ToList();
                if (!dispos.Any()) continue;

                // Récupérer uniquement les RDV confirmés pour ce médecin et cette date
                var rdvs = (await _rdvClient.GetRendezVousParMedecinEtDate(medecin.Id, request.date))
                    .Where(r => r.Statut == RDVstatus.CONFIRME)
                    .ToList();

                bool hasAvailableSlot = false;

                foreach (var dispo in dispos)
                {
                    var startDispo = request.date.Date + dispo.HeureDebut;
                    var endDispo = request.date.Date + dispo.HeureFin;

                    // Limiter à la plage demandée
                    if (request.heureDebut.HasValue && startDispo < request.date.Date + request.heureDebut.Value)
                        startDispo = request.date.Date + request.heureDebut.Value;

                    if (request.heureFin.HasValue && endDispo > request.date.Date + request.heureFin.Value)
                        endDispo = request.date.Date + request.heureFin.Value;

                    var current = startDispo;
                    while (current + DureeCreneau <= endDispo)
                    {
                        var slotStart = current;
                        var slotEnd = current + DureeCreneau;

                        bool chevauchement = rdvs.Any(rdv =>
                        {
                            var rdvStart = rdv.DateHeure;
                            var rdvEnd = rdvStart + DureeCreneau;
                            return !(slotEnd <= rdvStart || slotStart >= rdvEnd);
                        });

                        if (!chevauchement && slotStart > DateTime.Now)
                        {
                            hasAvailableSlot = true;
                            break; // au moins un créneau libre trouvé
                        }

                        current += DureeCreneau;
                    }

                    if (hasAvailableSlot) break;
                }

                if (hasAvailableSlot)
                    medecinsDisponibles.Add(medecin);
            }

            _logger.LogInformation("✅ {Count} médecins disponibles trouvés pour le {Date}",
                medecinsDisponibles.Count, request.date.ToShortDateString());

            return medecinsDisponibles;
        }
    }
}
