using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;

namespace PatientManagementService.Application.EventHandlers
{
    public class DocumentAttacheAuDossierHandler : INotificationHandler<DocumentAttacheAuDossier>
    {
        private readonly ILogger<DocumentAttacheAuDossierHandler> _logger;

        public DocumentAttacheAuDossierHandler(ILogger<DocumentAttacheAuDossierHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DocumentAttacheAuDossier notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"📎 Document '{notification.Document.Nom}' ajouté au dossier {notification.Document.DossierMedicalId}");
            return Task.CompletedTask;
        }
    }
}
