using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;
using PatientManagementService.Domain.Interfaces.Messaging;

namespace PatientManagementService.Application.EventHandlers
{
    public class DocumentAttacheAuDossierHandler : INotificationHandler<DocumentAttacheAuDossier>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<DocumentAttacheAuDossierHandler> _logger;

        public DocumentAttacheAuDossierHandler(IKafkaProducer producer, ILogger<DocumentAttacheAuDossierHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(DocumentAttacheAuDossier notification, CancellationToken cancellationToken)
        {
            // Publier l'événement DocumentAttacheAuDossier sur le topic Kafka
            await _producer.PublishAsync("document-attached", notification, cancellationToken);
            _logger.LogInformation($"📎 Document '{notification.Document.Nom}' ajouté au dossier {notification.Document.DossierMedicalId}");
        }
    }
}
