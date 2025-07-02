using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;
using PatientManagementService.Domain.Interfaces.Messaging;

namespace PatientManagementService.Application.EventHandlers
{
    public class DossierMedicalModifieHandler : INotificationHandler<DossierMedicalModifie>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DossierMedicalModifieHandler> _logger;

        public DossierMedicalModifieHandler(IKafkaProducer producer, ILogger<DossierMedicalModifieHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(DossierMedicalModifie notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("medicalRecord-updated", notification, cancellationToken);
            _logger.LogInformation($"📝 Dossier médical mis à jour (ID: {notification.DossierMedical.Id})");
        }
    }
}
