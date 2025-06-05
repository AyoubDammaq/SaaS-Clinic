using Doctor.Domain.Events.DisponibilityEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DisponibiliteEventHandlers
{
    public class DisponibiliteModifieeHandler : INotificationHandler<DisponibiliteModifiee>
    {
        private readonly ILogger<DisponibiliteModifieeHandler> _logger;

        public DisponibiliteModifieeHandler(ILogger<DisponibiliteModifieeHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DisponibiliteModifiee notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🟡✏️🕒 Disponibilité modifiée pour le médecin {MedecinId}, disponibilité ID : {DisponibiliteId}",
                notification.MedecinId, notification.DisponibiliteId);
            return Task.CompletedTask;
        }
    }
}
