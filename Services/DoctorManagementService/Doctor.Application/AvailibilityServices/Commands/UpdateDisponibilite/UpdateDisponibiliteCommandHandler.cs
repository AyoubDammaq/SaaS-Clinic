using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Commands.UpdateDisponibilite
{
    public class UpdateDisponibiliteCommandHandler : IRequestHandler<UpdateDisponibiliteCommand>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public UpdateDisponibiliteCommandHandler(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }
        public async Task Handle(UpdateDisponibiliteCommand request, CancellationToken cancellationToken)
        {
            if (request.disponibilite == null)
                throw new ArgumentNullException(nameof(request.disponibilite), "La disponibilité ne peut pas être null.");
            if (request.disponibilite.HeureDebut >= request.disponibilite.HeureFin)
                throw new ArgumentException("L'heure de début doit être inférieure à l'heure de fin.");

            request.disponibilite.ModifierDisponibiliteEvent();

            await _disponibiliteRepository.UpdateDisponibiliteAsync(request.disponibiliteId, request.disponibilite);
        }
    }
}
