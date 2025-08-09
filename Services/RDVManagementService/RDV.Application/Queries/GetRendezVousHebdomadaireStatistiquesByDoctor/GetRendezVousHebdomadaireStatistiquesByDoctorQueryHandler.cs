using MediatR;
using RDV.Application.DTOs;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetRendezVousHebdomadaireStatistiquesByDoctor
{
    public class GetRendezVousHebdomadaireStatistiquesByDoctorQueryHandler
    : IRequestHandler<GetRendezVousHebdomadaireStatistiquesByDoctorQuery, IEnumerable<RendezVousHebdoStatDto>>
    {
        private readonly IRendezVousRepository _rendezVousRepository;

        public GetRendezVousHebdomadaireStatistiquesByDoctorQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }

        public async Task<IEnumerable<RendezVousHebdoStatDto>> Handle(GetRendezVousHebdomadaireStatistiquesByDoctorQuery request, CancellationToken cancellationToken)
        {
            var rendezVous = await _rendezVousRepository.GetRendezVousByPeriod(request.DateDebut, request.DateFin);

            // Filtrage par médecin
            var filtered = rendezVous
                .Where(r => r.MedecinId == request.MedecinId)
                .ToList();

            var stats = rendezVous
                .GroupBy(r => r.DateHeure.DayOfWeek)
                .Select(g => new RendezVousHebdoStatDto
                {
                    Jour = g.Key.ToString(),
                    Scheduled = g.Count(),
                    Pending = g.Count(r => r.Statut == RDVstatus.EN_ATTENTE),
                    Cancelled = g.Count(r => r.Statut == RDVstatus.ANNULE)
                })
                .OrderBy(s => Convert.ToInt32(Enum.Parse(typeof(DayOfWeek), s.Jour))) 
                .ToList();

            return stats;
        }
    }
}
