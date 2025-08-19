using MediatR;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetNombreRendezVousByClinicAndToday
{
    public class GetNombreRendezVousByClinicAndTodayQueryHandler : IRequestHandler<GetNombreRendezVousByClinicAndTodayQuery, int>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public GetNombreRendezVousByClinicAndTodayQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<int> Handle(GetNombreRendezVousByClinicAndTodayQuery request, CancellationToken cancellationToken)
        {
            return await _rendezVousRepository.GetNombreRendezVousByClinicAndTodayAsync(request.ClinicId, request.Date);
        }
    }
}
