using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Commands.SupprimerDisponibilite
{
    public class SupprimerDisponibiliteCommandHandler : IRequestHandler<SupprimerDisponibiliteCommand>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public SupprimerDisponibiliteCommandHandler(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }
        public async Task Handle(SupprimerDisponibiliteCommand request, CancellationToken cancellationToken)
        {
            if (request.disponibiliteId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la disponibilité ne peut pas être vide.", nameof(request.disponibiliteId));

            var disponibilite = await _disponibiliteRepository.ObtenirDisponibiliteParIdAsync(request.disponibiliteId);
            if (disponibilite == null)
                throw new KeyNotFoundException("La disponibilité spécifiée n'existe pas.");

            await _disponibiliteRepository.SupprimerDisponibiliteAsync(request.disponibiliteId);
        }
    }
}
