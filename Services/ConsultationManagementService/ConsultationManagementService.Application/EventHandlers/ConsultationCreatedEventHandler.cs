using ConsultationManagementService.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Application.EventHandlers
{
    public class ConsultationCreatedEventHandler : INotificationHandler<ConsultationCreated>
    {
        private readonly ILogger<ConsultationCreatedEventHandler> _logger;

        public ConsultationCreatedEventHandler(ILogger<ConsultationCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ConsultationCreated notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"🟢 Consultation créée : {notification.Consultation.Id}");
            return Task.CompletedTask;
        }
    }
}
