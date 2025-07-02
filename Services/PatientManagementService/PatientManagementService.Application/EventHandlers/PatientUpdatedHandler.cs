using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;
using PatientManagementService.Domain.Interfaces.Messaging;

namespace PatientManagementService.Application.EventHandlers
{
    public class PatientUpdatedHandler : INotificationHandler<PatientUpdated>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<PatientUpdatedHandler> _logger;

        public PatientUpdatedHandler(IKafkaProducer producer, ILogger<PatientUpdatedHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(PatientUpdated notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("patient-updated", notification, cancellationToken);
            _logger.LogInformation("📝 Patient modifié : {Nom} {Prenom}", notification.Patient.Id, notification.Patient.Id);
        }
    }
}
