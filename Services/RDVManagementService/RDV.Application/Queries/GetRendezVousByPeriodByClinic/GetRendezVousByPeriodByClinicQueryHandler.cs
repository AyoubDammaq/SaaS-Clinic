using MediatR;
using RDV.Application.DTOs;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetRendezVousByPeriodByClinic
{
    public class GetRendezVousByPeriodByClinicQueryHandler : IRequestHandler<GetRendezVousByPeriodByClinicQuery, IEnumerable<RendezVousStatDTO>>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public GetRendezVousByPeriodByClinicQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<IEnumerable<RendezVousStatDTO>> Handle(GetRendezVousByPeriodByClinicQuery request, CancellationToken cancellationToken)
        {
            var rendezVous = await _rendezVousRepository.GetRendezVousByPeriodByClinicAsync(request.cliniqueId, request.start, request.end);
            var stats = rendezVous
                .GroupBy(r => r.DateHeure.Date)
                .Select(group => new RendezVousStatDTO
                {
                    Date = group.Key,
                    TotalRendezVous = group.Count(),
                    Confirmes = group.Count(r => r.Statut == RDVstatus.CONFIRME),
                    Annules = group.Count(r => r.Statut == RDVstatus.ANNULE),
                    EnAttente = group.Count(r => r.Statut == RDVstatus.EN_ATTENTE)
                })
                .OrderBy(s => s.Date)
                .ToList();
            return stats;
        }
    }
}
