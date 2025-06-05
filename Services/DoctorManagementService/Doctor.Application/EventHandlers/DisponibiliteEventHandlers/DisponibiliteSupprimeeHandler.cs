using Doctor.Domain.Events.DisponibilityEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DisponibiliteEventHandlers
{
    public class DisponibiliteSupprimeeHandler : INotificationHandler<DisponibiliteSupprimee>
    {
        private readonly ILogger<DisponibiliteSupprimeeHandler> _logger;

        public DisponibiliteSupprimeeHandler(ILogger<DisponibiliteSupprimeeHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DisponibiliteSupprimee notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning("🔴🗑️🕒 Disponibilité supprimée, ID : {DisponibiliteId}", notification.DisponibiliteId);
            return Task.CompletedTask;
        }
    }
}
