using Doctor.Domain.Events.DisponibilityEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DisponibiliteEventHandlers
{
    public class DisponibiliteAjouteeHandler : INotificationHandler<DisponibiliteAjoutee>
    {
        private readonly ILogger<DisponibiliteAjouteeHandler> _logger;

        public DisponibiliteAjouteeHandler(ILogger<DisponibiliteAjouteeHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DisponibiliteAjoutee notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🟢➕🕒 Disponibilité ajoutée pour le médecin {MedecinId}, disponibilité ID : {DisponibiliteId}",
                notification.MedecinId, notification.DisponibiliteId);
            return Task.CompletedTask;
        }
    }
}
