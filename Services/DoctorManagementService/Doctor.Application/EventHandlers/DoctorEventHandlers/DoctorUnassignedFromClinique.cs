using Doctor.Domain.Events;
using Doctor.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DoctorEventHandlers
{
    public class DoctorUnassignedFromCliniqueHandler : INotificationHandler<DoctorUnassignedFromClinique>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DoctorUnassignedFromCliniqueHandler> _logger;

        public DoctorUnassignedFromCliniqueHandler(IKafkaProducer producer, ILogger<DoctorUnassignedFromCliniqueHandler> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger;
        }

        public async Task Handle(DoctorUnassignedFromClinique notification, CancellationToken cancellationToken)
        {
            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("doctor-unassigned-to-clinic", notification, cancellationToken);

            _logger.LogWarning("👨‍⚕️➖🏥 Médecin désabonné d'une clinique : MedecinId = {MedecinId}, CliniqueId = {CliniqueId}",
                notification.MedecinId, notification.CliniqueId);
        }
    }
}
