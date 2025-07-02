using Doctor.Domain.Events.DisponibilityEvents;
using Doctor.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DisponibiliteEventHandlers
{
    public class DisponibiliteAjouteeHandler : INotificationHandler<DisponibiliteAjoutee>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DisponibiliteAjouteeHandler> _logger;

        public DisponibiliteAjouteeHandler(IKafkaProducer producer, ILogger<DisponibiliteAjouteeHandler> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger;
        }

        public async Task Handle(DisponibiliteAjoutee notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("availibility-added", notification, cancellationToken);

            _logger.LogInformation("🟢➕🕒 Disponibilité ajoutée pour le médecin {MedecinId}, disponibilité ID : {DisponibiliteId}",
                notification.MedecinId, notification.DisponibiliteId);
        }
    }
}
