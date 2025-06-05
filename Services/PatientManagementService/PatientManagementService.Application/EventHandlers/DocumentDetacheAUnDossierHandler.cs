using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;

namespace PatientManagementService.Application.EventHandlers
{
    public class DocumentDetacheAUnDossierHandler : INotificationHandler<DocumentDetacheAUnDossier>
    {
        private readonly ILogger<DocumentDetacheAUnDossierHandler> _logger;

        public DocumentDetacheAUnDossierHandler(ILogger<DocumentDetacheAUnDossierHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DocumentDetacheAUnDossier notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"📎❌ Document '{notification.Document.Nom}' supprimé du dossier {notification.Document.DossierMedicalId}");
            return Task.CompletedTask;
        }
    }
}
