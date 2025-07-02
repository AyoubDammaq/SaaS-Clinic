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
                throw new ArgumentException("HeureDebut must be before HeureFin.");

            // 🛡️ Vérification centralisée du chevauchement
            bool chevauche = await _disponibiliteRepository.VerifieChevauchementAsync(request.nouvelleDispo);
            if (chevauche)
                throw new InvalidOperationException("Ce créneau se chevauche avec une autre disponibilité existante.");

            request.nouvelleDispo.AjouterDisponibiliteEvent();

            await _disponibiliteRepository.AjouterDisponibiliteAsync(request.nouvelleDispo);
        }
    }
}
