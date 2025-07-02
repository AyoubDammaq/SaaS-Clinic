using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;
using PatientManagementService.Domain.Interfaces.Messaging;

namespace PatientManagementService.Application.EventHandlers
{
    public class DossierMedicalSupprimeHandler : INotificationHandler<DossierMedicalSupprime>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DossierMedicalSupprimeHandler> _logger;

        public DossierMedicalSupprimeHandler(IKafkaProducer producer, ILogger<DossierMedicalSupprimeHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(DossierMedicalSupprime notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("medicalRecord-deleted", notification, cancellationToken);
            _logger.LogInformation($"🗑️ Dossier médical supprimé (ID: {notification.DossierMedicalId})");
        }
    }
}
