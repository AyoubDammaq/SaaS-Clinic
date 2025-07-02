using Doctor.Domain.Events.DisponibilityEvents;
using Doctor.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DisponibiliteEventHandlers
{
    public class DisponibiliteModifieeHandler : INotificationHandler<DisponibiliteModifiee>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DisponibiliteModifieeHandler> _logger;

        public DisponibiliteModifieeHandler(IKafkaProducer producer, ILogger<DisponibiliteModifieeHandler> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger;
        }

        public async Task Handle(DisponibiliteModifiee notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("availibility-updated", notification, cancellationToken);

            _logger.LogInformation("🟡✏️🕒 Disponibilité modifiée pour le médecin {MedecinId}, disponibilité ID : {DisponibiliteId}",
                notification.MedecinId, notification.DisponibiliteId);
        }
    }
}
