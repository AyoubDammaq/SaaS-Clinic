using Doctor.Application.DTOs;
using Doctor.Application.Interfaces;
using Doctor.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.AvailibilityServices.Queries.GetCreneauxDisponibles
{
    public class GetCreneauxDisponiblesQueryHandler : IRequestHandler<GetCreneauxDisponiblesQuery, List<CreneauDisponibleDto>>
    {
        private readonly IDisponibiliteRepository _dispoRepo;
        private readonly IRendezVousHttpClient _rdvClient;
        private readonly ILogger<GetCreneauxDisponiblesQueryHandler> _logger;



        private static readonly TimeSpan DureeCreneau = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan Buffer = TimeSpan.FromMinutes(10);

        public GetCreneauxDisponiblesQueryHandler(IDisponibiliteRepository dispoRepo, IRendezVousHttpClient rdvClient, ILogger<GetCreneauxDisponiblesQueryHandler> logger)
        {
            _dispoRepo = dispoRepo;
            _rdvClient = rdvClient;
            _logger = logger;
        }

        public async Task<List<CreneauDisponibleDto>> Handle(GetCreneauxDisponiblesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔍 Traitement de la requête de créneaux disponibles pour le médecin {MedecinId} à la date {Date}", request.MedecinId, request.Date);

            var date = request.Date.Date;
            var jour = date.DayOfWeek;

            var dispos = await _dispoRepo.ObtenirDisponibilitesParJourAsync(request.MedecinId, jour);
            _logger.LogInformation("📅 {Count} disponibilités récupérées pour le jour {Jour} du médecin {MedecinId}", dispos.Count, jour, request.MedecinId);

            var rdvs = await _rdvClient.GetRendezVousParMedecinEtDate(request.MedecinId, request.Date);
            _logger.LogInformation("📋 {Count} rendez-vous existants récupérés pour le médecin {MedecinId} le {Date}", rdvs.Count, request.MedecinId, request.Date.ToShortDateString());

            var creneauxDisponibles = new List<CreneauDisponibleDto>();

            foreach (var dispo in dispos)
            {
                var heureDebut = date.Add(dispo.HeureDebut);
                var heureFin = date.Add(dispo.HeureFin);

                _logger.LogDebug("🕒 Disponibilité de {Start} à {End}", heureDebut, heureFin);

                var current = heureDebut;
                while (current.Add(DureeCreneau) <= heureFin)
                {
                    var slotStart = current;
                    var slotEnd = current.Add(DureeCreneau);
                    var slotWithBufferEnd = slotEnd.Add(Buffer);

                    bool chevauchement = rdvs.Any(rdv =>
                    {
                        var rdvStart = rdv.DateHeure;
                        var rdvEnd = rdvStart.Add(DureeCreneau);
                        return !(slotWithBufferEnd <= rdvStart || slotStart >= rdvEnd);
                    });

                    if (!chevauchement && slotStart > DateTime.Now)
                    {
                        _logger.LogDebug("✅ Créneau disponible : {Start} → {End}", slotStart, slotEnd);
                        creneauxDisponibles.Add(new CreneauDisponibleDto
                        {
                            DateHeureDebut = slotStart,
                            DateHeureFin = slotEnd
                        });
                    }
                    else
                    {
                        _logger.LogDebug("⛔ Créneau rejeté (conflit ou passé) : {Start} → {End}", slotStart, slotEnd);
                    }

                    current = current.Add(DureeCreneau);
                }
            }

            _logger.LogInformation("✅ {Count} créneaux disponibles générés pour le médecin {MedecinId}", creneauxDisponibles.Count, request.MedecinId);

            return creneauxDisponibles;
        }
    }
}
