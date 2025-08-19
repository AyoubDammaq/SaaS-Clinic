using MediatR;
using RDV.Application.DTOs;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetRendezVousByPeriodByPatient
{
    public class GetRendezVousByPeriodByPatientQueryHandler : IRequestHandler<GetRendezVousByPeriodByPatientQuery, IEnumerable<RendezVousStatDTO>>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public GetRendezVousByPeriodByPatientQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<IEnumerable<RendezVousStatDTO>> Handle(GetRendezVousByPeriodByPatientQuery request, CancellationToken cancellationToken)
        {
            var rendezVous = await _rendezVousRepository.GetRendezVousByPeriodByPatientAsync(request.PatientId, request.Start, request.End);
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
