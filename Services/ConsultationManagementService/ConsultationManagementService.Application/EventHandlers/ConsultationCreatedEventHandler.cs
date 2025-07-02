using ConsultationManagementService.Domain.Events;
using ConsultationManagementService.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Application.EventHandlers
{
    public class ConsultationCreatedEventHandler : INotificationHandler<ConsultationCreated>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<ConsultationCreatedEventHandler> _logger;

        public ConsultationCreatedEventHandler(IKafkaProducer producer, ILogger<ConsultationCreatedEventHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(ConsultationCreated notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("consultation-created", notification, cancellationToken);
            _logger.LogInformation($"🟢 Consultation créée : {notification.Consultation.Id}");
        }
    }
}
