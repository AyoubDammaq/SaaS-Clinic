using MediatR;
using Microsoft.Extensions.Logging;
using RDV.Domain.Events;
using RDV.Domain.Interfaces.Messaging;

namespace RDV.Application.EventHandlers
{
    public class RendezVousModifieHandler : INotificationHandler<RendezVousModifie>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<RendezVousModifieHandler> _logger;

        public RendezVousModifieHandler(IKafkaProducer producer, ILogger<RendezVousModifieHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(RendezVousModifie notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("rdv-updated", notification, cancellationToken);

            _logger.LogInformation($"📝 Rendez-vous modifié : {notification.RendezVous.Id}, Nouvelle date : {notification.RendezVous.DateHeure}, Commentaire : {notification.RendezVous.Commentaire}");
        }
    }
}
