using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;

namespace PatientManagementService.Application.EventHandlers
{
    public class DossierMedicalCreeHandler : INotificationHandler<DossierMedicalCree>
    {
        private readonly ILogger<DossierMedicalCreeHandler> _logger;

        public DossierMedicalCreeHandler(ILogger<DossierMedicalCreeHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DossierMedicalCree notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"🟢 Dossier médical créé pour le patient {notification.DossierMedical.Patient.Nom} {notification.DossierMedical.Patient.Prenom}, ID Dossier: {notification.DossierMedical.Id}");
            return Task.CompletedTask;
        }
    }
}
