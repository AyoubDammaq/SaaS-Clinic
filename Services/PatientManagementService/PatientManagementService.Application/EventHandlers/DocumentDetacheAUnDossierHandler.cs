using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;
using PatientManagementService.Domain.Interfaces.Messaging;

namespace PatientManagementService.Application.EventHandlers
{
    public class DocumentDetacheAUnDossierHandler : INotificationHandler<DocumentDetacheAUnDossier>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DocumentDetacheAUnDossierHandler> _logger;

        public DocumentDetacheAUnDossierHandler(IKafkaProducer producer, ILogger<DocumentDetacheAUnDossierHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(DocumentDetacheAUnDossier notification, CancellationToken cancellationToken)
        {
            // Publier l'événement DocumentDetacheAUnDossier sur le topic Kafka
            await _producer.PublishAsync("document-detached", notification, cancellationToken);
            _logger.LogInformation($"📎❌ Document '{notification.Document.Nom}' supprimé du dossier {notification.Document.DossierMedicalId}");
        }
    }
}
