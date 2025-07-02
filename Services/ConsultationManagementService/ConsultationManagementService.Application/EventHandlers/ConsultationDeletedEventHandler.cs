using ConsultationManagementService.Domain.Events;
using ConsultationManagementService.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Application.EventHandlers
{
    public class ConsultationDeletedEventHandler : INotificationHandler<ConsultationDeleted>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<ConsultationDeletedEventHandler> _logger;

        public ConsultationDeletedEventHandler(IKafkaProducer producer, ILogger<ConsultationDeletedEventHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(ConsultationDeleted notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("consultation-deleted", notification, cancellationToken);
            _logger.LogInformation($"🗑️ Consultation supprimée : {notification.Consultation.Id}");
        }
    }
}
