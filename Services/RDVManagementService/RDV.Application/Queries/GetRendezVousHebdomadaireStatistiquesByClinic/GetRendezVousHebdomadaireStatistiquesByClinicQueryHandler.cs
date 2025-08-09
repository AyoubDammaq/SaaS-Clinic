using MediatR;
using RDV.Application.DTOs;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;
using System.Text.Json;

namespace RDV.Application.Queries.GetRendezVousHebdomadaireStatistiquesByClinic
{
    public class GetRendezVousHebdomadaireStatistiquesByClinicQueryHandler
    : IRequestHandler<GetRendezVousHebdomadaireStatistiquesByClinicQuery, IEnumerable<RendezVousHebdoStatDto>>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetRendezVousHebdomadaireStatistiquesByClinicQueryHandler(IRendezVousRepository rendezVousRepository, IHttpClientFactory httpClientFactory)
        {
            _rendezVousRepository = rendezVousRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<RendezVousHebdoStatDto>> Handle(GetRendezVousHebdomadaireStatistiquesByClinicQuery request, CancellationToken cancellationToken)
        {
            // Récupérer les IDs des médecins de cette clinique
            var medecinsIds = await GetMedecinIdsByCliniqueAsync(request.CliniqueId);

            if (medecinsIds == null || !medecinsIds.Any())
                return Enumerable.Empty<RendezVousHebdoStatDto>();

            // Récupérer tous les rendez-vous dans la période
            var rendezVous = await _rendezVousRepository.GetRendezVousByPeriod(request.DateDebut, request.DateFin);

            // Filtrer les rendez-vous par médecins de cette clinique
            var filtered = rendezVous
                .Where(r => medecinsIds.Contains(r.MedecinId))
                .ToList();

            // Grouper par jour et calculer les statistiques
            var stats = filtered
                .GroupBy(r => r.DateHeure.DayOfWeek)
                .Select(g => new RendezVousHebdoStatDto
                {
                    Jour = g.Key.ToString(),
                    Scheduled = g.Count(),
                    Pending = g.Count(r => r.Statut == RDVstatus.EN_ATTENTE),
                    Cancelled = g.Count(r => r.Statut == RDVstatus.ANNULE)
                })
                .OrderBy(s => (int)Enum.Parse(typeof(DayOfWeek), s.Jour))
                .ToList();

            return stats;
        }


        private async Task<List<Guid>> GetMedecinIdsByCliniqueAsync(Guid cliniqueId)
        {
            var client = _httpClientFactory.CreateClient(); // Doit être injecté via DI
            var response = await client.GetAsync($"http://doctorservice:8085/api/Medecin/medecinsIds/clinique/{cliniqueId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erreur lors de la récupération des IDs des médecins depuis DoctorService");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Guid>>(json)!;
        }

    }
}
