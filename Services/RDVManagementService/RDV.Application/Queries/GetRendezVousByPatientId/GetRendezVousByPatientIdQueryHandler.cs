using MediatR;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetRendezVousByPatientId
{
    public class GetRendezVousByPatientIdQueryHandler : IRequestHandler<GetRendezVousByPatientIdQuery, IEnumerable<RendezVous>>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public GetRendezVousByPatientIdQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<IEnumerable<RendezVous>> Handle(GetRendezVousByPatientIdQuery request, CancellationToken cancellationToken)
        {
            if (request.patientId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du patient ne peut pas être vide.", nameof(request.patientId));
            }
            return await _rendezVousRepository.GetRendezVousByPatientIdAsync(request.patientId);
        }
    }
}
