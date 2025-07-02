using Facturation.Domain.Events;
using Facturation.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.EventHandlers
{
    public class FactureCreatedEventHandler : INotificationHandler<FactureCreated>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<FactureCreatedEventHandler> _logger;

        public FactureCreatedEventHandler(IKafkaProducer producer, ILogger<FactureCreatedEventHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(FactureCreated notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("facture-created", notification, cancellationToken);

            _logger.LogInformation("✅ Nouvelle facture créée : {Id} de montant : {MontantTotal} $", notification.Facture.Id, notification.Facture.MontantTotal);
        }
    }
}
