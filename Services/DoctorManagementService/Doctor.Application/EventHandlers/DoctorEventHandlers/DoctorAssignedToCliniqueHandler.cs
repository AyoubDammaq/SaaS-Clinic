using Doctor.Domain.Events;
using Doctor.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DoctorEventHandlers
{
    public class DoctorAssignedToCliniqueHandler : INotificationHandler<DoctorAssignedToClinique>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DoctorAssignedToCliniqueHandler> _logger;

        public DoctorAssignedToCliniqueHandler(IKafkaProducer producer, ILogger<DoctorAssignedToCliniqueHandler> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger;
        }

        public async Task Handle(DoctorAssignedToClinique notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("doctor-assigned-to-clinic", notification, cancellationToken);

            _logger.LogInformation("👨‍⚕️➕🏥 Médecin assigné à une clinique : MedecinId = {MedecinId}, CliniqueId = {CliniqueId}",
                notification.MedecinId, notification.CliniqueId);
        }
    }
}
