using Doctor.Domain.Events;
using Doctor.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DoctorEventHandlers
{
    public class DoctorCreatedHandler : INotificationHandler<DoctorCreated>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DoctorCreatedHandler> _logger;
        public DoctorCreatedHandler(IKafkaProducer producer, ILogger<DoctorCreatedHandler> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DoctorCreated notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("doctor-created", notification, cancellationToken);

            _logger.LogInformation("✅ Doctor created: Dr.{Nom} {Prenom}", notification.Medecin.Nom, notification.Medecin.Prenom);
        }
    }
}
