using MediatR;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetRendezVousById
{
    public class GetRendezVousByIdQueryHandler : IRequestHandler<GetRendezVousByIdQuery, RendezVous>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public GetRendezVousByIdQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<RendezVous> Handle(GetRendezVousByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du rendez-vous ne peut pas être vide.", nameof(request.id));
            }
            return await _rendezVousRepository.GetRendezVousByIdAsync(request.id);
        }
    }
}
