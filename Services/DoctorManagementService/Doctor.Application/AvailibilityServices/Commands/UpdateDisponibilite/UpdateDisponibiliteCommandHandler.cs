using Doctor.Domain.Entities;
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
            var dispo = request.disponibilite;

            if (dispo == null)
                throw new ArgumentNullException(nameof(dispo), "La disponibilité ne peut pas être null.");

            if (dispo.HeureDebut >= dispo.HeureFin)
                throw new ArgumentException("L'heure de début doit être inférieure à l'heure de fin.");

            dispo.Id = request.disponibiliteId; // nécessaire pour exclure l'élément actuel dans le check

            // 🛡️ Vérification centralisée du chevauchement
            bool chevauche = await _disponibiliteRepository.VerifieChevauchementAsync(dispo);
            if (chevauche)
                throw new InvalidOperationException("Ce créneau se chevauche avec une autre disponibilité existante.");

            dispo.ModifierDisponibiliteEvent();

            await _disponibiliteRepository.UpdateDisponibiliteAsync(request.disponibiliteId, dispo);
        }
    }
}
