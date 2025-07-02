using MediatR;
using Microsoft.Extensions.Logging;
using RDV.Domain.Events;
using RDV.Domain.Interfaces.Messaging;

namespace RDV.Application.EventHandlers
{
    public class RendezVousAnnuleParMedecinHandler : INotificationHandler<RendezVousAnnuleParMedecin>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<RendezVousAnnuleParMedecinHandler> _logger;

        public RendezVousAnnuleParMedecinHandler(IKafkaProducer producer, ILogger<RendezVousAnnuleParMedecinHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(RendezVousAnnuleParMedecin notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("rdv-cancelled-by-doctor", notification, cancellationToken);

            _logger.LogInformation($"❌📎 Rendez-vous annulé : {notification.RendezVous.Id}, Raison: {notification.Raison}");
        }
    }
}
