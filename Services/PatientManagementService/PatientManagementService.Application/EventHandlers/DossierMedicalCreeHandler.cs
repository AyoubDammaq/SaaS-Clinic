using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;
using PatientManagementService.Domain.Interfaces.Messaging;

namespace PatientManagementService.Application.EventHandlers
{
    public class DossierMedicalCreeHandler : INotificationHandler<DossierMedicalCree>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DossierMedicalCreeHandler> _logger;

        public DossierMedicalCreeHandler(IKafkaProducer producer, ILogger<DossierMedicalCreeHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(DossierMedicalCree notification, CancellationToken cancellationToken)
        {
            // Publier l'événement DossierMedicalCree sur le topic Kafka
            await _producer.PublishAsync("medicalRecord-created", notification, cancellationToken);
            _logger.LogInformation($"🟢 Dossier médical créé pour le patient {notification.DossierMedical.Patient.Nom} {notification.DossierMedical.Patient.Prenom}, ID Dossier: {notification.DossierMedical.Id}");
        }
    }
}
