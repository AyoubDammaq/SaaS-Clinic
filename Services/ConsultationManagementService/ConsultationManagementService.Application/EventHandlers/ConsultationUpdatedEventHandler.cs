using ConsultationManagementService.Domain.Events;
using ConsultationManagementService.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Application.EventHandlers
{
    public class ConsultationUpdatedEventHandler : INotificationHandler<ConsultationUpdated>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<ConsultationUpdatedEventHandler> _logger;

        public ConsultationUpdatedEventHandler(IKafkaProducer producer, ILogger<ConsultationUpdatedEventHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(ConsultationUpdated notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("consultation-updated", notification, cancellationToken);
            _logger.LogInformation($"📝 Consultation mise à jour : {notification.Consultation.Id}");
        }
    }
}
