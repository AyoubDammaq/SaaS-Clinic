using MediatR;
using Microsoft.Extensions.Logging;
using RDV.Domain.Events;
using RDV.Domain.Interfaces.Messaging;

namespace RDV.Application.EventHandlers
{
    public class RendezVousAnnuleHandler : INotificationHandler<RendezVousAnnule>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<RendezVousAnnuleHandler> _logger;

        public RendezVousAnnuleHandler(IKafkaProducer producer, ILogger<RendezVousAnnuleHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(RendezVousAnnule notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("rdv-cancelled", notification, cancellationToken);

            _logger.LogInformation($"❌ Rendez-vous annulé : {notification.RendezVous.Id} par le patient");
        }
    }
}
