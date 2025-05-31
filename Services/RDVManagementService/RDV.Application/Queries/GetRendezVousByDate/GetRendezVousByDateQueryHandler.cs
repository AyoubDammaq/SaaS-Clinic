using MediatR;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetRendezVousByDate
{
    public class GetRendezVousByDateQueryHandler : IRequestHandler<GetRendezVousByDateQuery, IEnumerable<RendezVous>>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public GetRendezVousByDateQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<IEnumerable<RendezVous>> Handle(GetRendezVousByDateQuery request, CancellationToken cancellationToken)
        {
            return await _rendezVousRepository.GetRendezVousByDateAsync(request.date);
        }
    }
}
