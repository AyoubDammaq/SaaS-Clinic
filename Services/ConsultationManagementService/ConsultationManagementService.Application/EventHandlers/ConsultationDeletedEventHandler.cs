using ConsultationManagementService.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Application.EventHandlers
{
    public class ConsultationDeletedEventHandler : INotificationHandler<ConsultationDeleted>
    {
        private readonly ILogger<ConsultationDeletedEventHandler> _logger;

        public ConsultationDeletedEventHandler(ILogger<ConsultationDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ConsultationDeleted notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"🗑️ Consultation supprimée : {notification.Consultation.Id}");
            return Task.CompletedTask;
        }

    }
}
