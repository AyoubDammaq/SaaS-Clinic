using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Commands.AjouterDisponibilite
{
    public class AjouterDisponibiliteCommandHandler : IRequestHandler<AjouterDisponibiliteCommand>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public AjouterDisponibiliteCommandHandler(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }
        public async Task Handle(AjouterDisponibiliteCommand request, CancellationToken cancellationToken)
        {
            if (request.nouvelleDispo == null)
                throw new ArgumentNullException(nameof(request.nouvelleDispo), "La disponibilité ne peut pas être null.");

            if (request.nouvelleDispo.HeureDebut >= request.nouvelleDispo.HeureFin)
                throw new ArgumentException("L'heure de début doit être inférieure à l'heure de fin.");

            await _disponibiliteRepository.AjouterDisponibiliteAsync(request.nouvelleDispo);
        }
    }
}
