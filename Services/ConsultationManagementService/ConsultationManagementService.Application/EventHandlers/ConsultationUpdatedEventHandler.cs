using ConsultationManagementService.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Application.EventHandlers
{
    public class ConsultationUpdatedEventHandler : INotificationHandler<ConsultationUpdated>
    {
        private readonly ILogger<ConsultationUpdatedEventHandler> _logger;

        public ConsultationUpdatedEventHandler(ILogger<ConsultationUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ConsultationUpdated notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"📝 Consultation mise à jour : {notification.Consultation.Id}");
            return Task.CompletedTask;
        }
    }
}
