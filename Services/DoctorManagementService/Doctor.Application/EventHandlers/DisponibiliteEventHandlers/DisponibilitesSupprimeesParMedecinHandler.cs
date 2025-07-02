using Doctor.Domain.Events.DisponibilityEvents;
using Doctor.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DisponibiliteEventHandlers
{
    public class DisponibilitesSupprimeesParMedecinHandler : INotificationHandler<DisponibilitesSupprimeesParMedecin>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DisponibilitesSupprimeesParMedecinHandler> _logger;

        public DisponibilitesSupprimeesParMedecinHandler(IKafkaProducer producer, ILogger<DisponibilitesSupprimeesParMedecinHandler> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger;
        }

        public async Task Handle(DisponibilitesSupprimeesParMedecin notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("availibilities-deleted-for-a-doctor", notification, cancellationToken);

            _logger.LogWarning("❌🗑️🕒 Toutes les disponibilités du médecin {MedecinId} ont été supprimées.", notification.MedecinId);
        }
    }
}
