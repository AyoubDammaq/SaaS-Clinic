using MediatR;
using Microsoft.Extensions.Logging;
using RDV.Domain.Events;
using RDV.Domain.Interfaces.Messaging;

namespace RDV.Application.EventHandlers
{
    public class RendezVousConfirmeHandler : INotificationHandler<RendezVousConfirmed>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<RendezVousConfirmeHandler> _logger;

        public RendezVousConfirmeHandler(IKafkaProducer producer, ILogger<RendezVousConfirmeHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(RendezVousConfirmed notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("rdv-confirmed", notification, cancellationToken);

            _logger.LogInformation($"✅ Rendez-vous confirmé : {notification.RendezVous.Id}");

        }
    }
}
