using Facturation.Domain.Events;
using Facturation.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.EventHandlers
{
    public class FactureDeletedEventHandler : INotificationHandler<FactureDeleted>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<FactureDeletedEventHandler> _logger;

        public FactureDeletedEventHandler(IKafkaProducer producer, ILogger<FactureDeletedEventHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(FactureDeleted notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("facture-deleted", notification, cancellationToken);

            _logger.LogInformation("🗑️ Facture supprimée : {FactureId}", notification.FactureId);
        }
    }
}
