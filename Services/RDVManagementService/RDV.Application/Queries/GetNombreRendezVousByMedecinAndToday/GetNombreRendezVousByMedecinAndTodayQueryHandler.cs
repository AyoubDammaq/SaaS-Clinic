using MediatR;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetNombreRendezVousByMedecinAndToday
{
    public class GetNombreRendezVousByMedecinAndTodayQueryHandler : IRequestHandler<GetNombreRendezVousByMedecinAndTodayQuery, int>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public GetNombreRendezVousByMedecinAndTodayQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<int> Handle(GetNombreRendezVousByMedecinAndTodayQuery request, CancellationToken cancellationToken)
        {
            return await _rendezVousRepository.GetNombreRendezVousByMedecinAndTodayAsync(request.MedecinId, request.Date);
        }
    }
}