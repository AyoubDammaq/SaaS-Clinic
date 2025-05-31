using MediatR;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetRendezVousByStatut
{
    public class GetRendezVousByStatutQueryHandler : IRequestHandler<GetRendezVousByStatutQuery, IEnumerable<RendezVous>>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public GetRendezVousByStatutQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<IEnumerable<RendezVous>> Handle(GetRendezVousByStatutQuery request, CancellationToken cancellationToken)
        {
            return await _rendezVousRepository.GetRendezVousByStatutAsync(request.statut);
        }
    }
}
