using Doctor.Domain.Events.DisponibilityEvents;
using Doctor.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DisponibiliteEventHandlers
{
    public class DisponibiliteSupprimeeHandler : INotificationHandler<DisponibiliteSupprimee>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DisponibiliteSupprimeeHandler> _logger;

        public DisponibiliteSupprimeeHandler(IKafkaProducer producer, ILogger<DisponibiliteSupprimeeHandler> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger;
        }

        public async Task Handle(DisponibiliteSupprimee notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("availibility-deleted", notification, cancellationToken);

            _logger.LogWarning("🔴🗑️🕒 Disponibilité supprimée, ID : {DisponibiliteId}", notification.DisponibiliteId);
        }
    }
}
