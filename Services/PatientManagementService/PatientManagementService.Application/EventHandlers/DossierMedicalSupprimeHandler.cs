using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;

namespace PatientManagementService.Application.EventHandlers
{
    public class DossierMedicalSupprimeHandler : INotificationHandler<DossierMedicalSupprime>
    {
        private readonly ILogger<DossierMedicalSupprimeHandler> _logger;

        public DossierMedicalSupprimeHandler(ILogger<DossierMedicalSupprimeHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DossierMedicalSupprime notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"🗑️ Dossier médical supprimé (ID: {notification.DossierMedicalId})");
            return Task.CompletedTask;
        }
    }
}
