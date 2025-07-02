using MediatR;
using Microsoft.Extensions.Logging;
using Doctor.Domain.Events;
using Doctor.Domain.Interfaces.Messaging;

namespace Doctor.Application.EventHandlers.DoctorEventHandlers
{
    public class DoctorRemovedHandler : INotificationHandler<DoctorRemoved>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DoctorRemovedHandler> _logger;
        public DoctorRemovedHandler(IKafkaProducer producer, ILogger<DoctorRemovedHandler> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DoctorRemoved notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("doctor-removed", notification, cancellationToken);

            _logger.LogInformation("🗑️ Doctor removed: Dr.{Nom} {Prenom}", notification.Medecin.Nom, notification.Medecin.Prenom);
        }
    }
}
