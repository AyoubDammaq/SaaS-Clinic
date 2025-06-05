using Doctor.Domain.Entities;
using Doctor.Domain.Events.DisponibilityEvents;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Commands.SupprimerDisponibilitesParMedecinId
{
    public class SupprimerDisponibilitesParMedecinIdCommandHandler : IRequestHandler<SupprimerDisponibilitesParMedecinIdCommand>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public SupprimerDisponibilitesParMedecinIdCommandHandler(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }
        public async Task Handle(SupprimerDisponibilitesParMedecinIdCommand request, CancellationToken cancellationToken)
        {
            if (request.medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(request.medecinId));

            var disponibilites = await _disponibiliteRepository.ObtenirDisponibilitesParMedecinIdAsync(request.medecinId);

            Disponibilite.SupprimerToutesPourMedecinEvent(request.medecinId);

            await _disponibiliteRepository.SupprimerDisponibilitesParMedecinIdAsync(request.medecinId);
        }
    }
}
