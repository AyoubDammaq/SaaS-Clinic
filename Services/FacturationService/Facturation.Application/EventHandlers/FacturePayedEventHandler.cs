using Facturation.Application.DTOs;
using Facturation.Domain.Events;
using Facturation.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.EventHandlers
{
    public class FacturePayedEventHandler : INotificationHandler<FacturePayed>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<FacturePayedEventHandler> _logger;

        public FacturePayedEventHandler(IKafkaProducer producer, ILogger<FacturePayedEventHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(FacturePayed notification, CancellationToken cancellationToken)
        {
            var paiement = notification.Paiement;

            var dto = new FacturePayedDTO
            {
                PaiementId = paiement.Id,
                Montant = paiement.Montant,
                DatePaiement = paiement.DatePaiement,
                Mode = paiement.Mode,
                FactureId = paiement.FactureId,
            };

            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("facture-payed", dto, cancellationToken);

            _logger.LogInformation("💰 Paiement effectué : {Id} pour la facture : {FactureId}", notification.Paiement.Id, notification.Paiement.FactureId);
        }
    }
}
