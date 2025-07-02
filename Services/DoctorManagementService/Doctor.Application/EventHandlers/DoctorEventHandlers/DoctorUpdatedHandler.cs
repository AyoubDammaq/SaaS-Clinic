using MediatR;
using Microsoft.Extensions.Logging;
using Doctor.Domain.Events;
using Doctor.Domain.Interfaces.Messaging;

namespace Doctor.Application.EventHandlers.DoctorEventHandlers
{
    public class DoctorUpdatedHandler : INotificationHandler<DoctorUpdated>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DoctorUpdatedHandler> _logger;
        public DoctorUpdatedHandler(IKafkaProducer producer, ILogger<DoctorUpdatedHandler> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DoctorUpdated notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("doctor-updated", notification, cancellationToken);

            _logger.LogInformation("📝 Doctor updated: Dr.{Nom} {Prenom}", notification.Medecin.Nom, notification.Medecin.Prenom);
        }
    }
}
