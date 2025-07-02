using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;
using PatientManagementService.Domain.Interfaces.Messaging;

namespace PatientManagementService.Application.EventHandlers
{
    public class PatientDeletedHandler : INotificationHandler<PatientDeleted>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<PatientDeletedHandler> _logger;

        public PatientDeletedHandler(IKafkaProducer producer, ILogger<PatientDeletedHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }
        public async Task Handle(PatientDeleted notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("patient-deleted", notification, cancellationToken);
            _logger.LogInformation("🗑️ Patient supprimé : {PatientId}", notification.PatientId);
        }
    }
}
