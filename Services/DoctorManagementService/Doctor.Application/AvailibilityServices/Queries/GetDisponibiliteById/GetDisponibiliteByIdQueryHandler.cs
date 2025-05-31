using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetDisponibiliteById
{
    public class GetDisponibiliteByIdQueryHandler : IRequestHandler<GetDisponibiliteByIdQuery, Disponibilite>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public GetDisponibiliteByIdQueryHandler(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }
        public async Task<Disponibilite> Handle(GetDisponibiliteByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.disponibiliteId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la disponibilité ne peut pas être vide.", nameof(request.disponibiliteId));

            var disponibilite = await _disponibiliteRepository.ObtenirDisponibiliteParIdAsync(request.disponibiliteId);

            return disponibilite ?? new Disponibilite();
        }
    }
}
