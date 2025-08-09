using MediatR;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.CountPendingRDVByDoctor
{
    public class CountPendingRDVByDoctorQueryHandler : IRequestHandler<CountPendingRDVByDoctorQuery, int>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public CountPendingRDVByDoctorQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<int> Handle(CountPendingRDVByDoctorQuery request, CancellationToken cancellationToken)
        {
            return await _rendezVousRepository.CountPendingRDVByDoctorAsync(request.medecinId);
        }
    }
}
