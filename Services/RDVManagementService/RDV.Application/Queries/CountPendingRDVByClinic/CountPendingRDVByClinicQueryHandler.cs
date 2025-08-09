using MediatR;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.CountPendingRDVByClinic
{
    public class CountPendingRDVByClinicQueryHandler : IRequestHandler<CountPendingRDVByClinicQuery, int>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public CountPendingRDVByClinicQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<int> Handle(CountPendingRDVByClinicQuery request, CancellationToken cancellationToken)
        {
            return await _rendezVousRepository.CountPendingRDVByClinicAsync(request.clinicId);
        }
    }
}
