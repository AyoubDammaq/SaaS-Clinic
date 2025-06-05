using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;

namespace PatientManagementService.Application.EventHandlers
{
    public class DossierMedicalModifieHandler : INotificationHandler<DossierMedicalModifie>
    {
        private readonly ILogger<DossierMedicalModifieHandler> _logger;

        public DossierMedicalModifieHandler(ILogger<DossierMedicalModifieHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DossierMedicalModifie notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"📝 Dossier médical mis à jour (ID: {notification.DossierMedical.Id})");
            return Task.CompletedTask;
        }
    }
}
