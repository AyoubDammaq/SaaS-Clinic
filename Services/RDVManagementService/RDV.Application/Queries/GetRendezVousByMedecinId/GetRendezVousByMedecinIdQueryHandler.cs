using MediatR;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetRendezVousByMedecinId
{
    public class GetRendezVousByMedecinIdQueryHandler : IRequestHandler<GetRendezVousByMedecinIdQuery, IEnumerable<RendezVous>>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public GetRendezVousByMedecinIdQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<IEnumerable<RendezVous>> Handle(GetRendezVousByMedecinIdQuery request, CancellationToken cancellationToken)
        {
            if (request.medecinId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(request.medecinId));
            }
            return await _rendezVousRepository.GetRendezVousByMedecinIdAsync(request.medecinId);
        }
    }
}