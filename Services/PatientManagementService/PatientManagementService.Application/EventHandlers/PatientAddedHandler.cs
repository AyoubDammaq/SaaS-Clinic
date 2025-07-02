using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;
using PatientManagementService.Domain.Interfaces.Messaging;

namespace PatientManagementService.Application.EventHandlers
{
    public class PatientAddedHandler : INotificationHandler<PatientAdded>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<PatientAddedHandler> _logger;

        public PatientAddedHandler(IKafkaProducer producer, ILogger<PatientAddedHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(PatientAdded notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("patient-added", notification, cancellationToken);

            _logger.LogInformation("✅ Nouveau patient ajouté : {Nom} {Prenom}", notification.Nom, notification.Prenom);
        }
    }
}
