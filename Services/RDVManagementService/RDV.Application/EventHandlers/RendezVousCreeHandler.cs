using MediatR;
using Microsoft.Extensions.Logging;
using RDV.Domain.Events;
using RDV.Domain.Interfaces.Messaging;

namespace RDV.Application.EventHandlers
{
    public class RendezVousCreeHandler : INotificationHandler<RendezVousCree>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<RendezVousCreeHandler> _logger;

        public RendezVousCreeHandler(IKafkaProducer producer, ILogger<RendezVousCreeHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(RendezVousCree notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("rdv-created", notification, cancellationToken);

            _logger.LogInformation($"🆕 Rendez-vous créé : {notification.RendezVous.Id}, Médecin: {notification.RendezVous.MedecinId}, Patient: {notification.RendezVous.PatientId}, Date: {notification.RendezVous.DateHeure}");

        }
    }
}
