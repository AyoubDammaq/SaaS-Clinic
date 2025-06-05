using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;

namespace PatientManagementService.Application.EventHandlers
{
    public class PatientUpdatedHandler : INotificationHandler<PatientUpdated>
    {
        private readonly ILogger<PatientUpdatedHandler> _logger;

        public PatientUpdatedHandler(ILogger<PatientUpdatedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(PatientUpdated notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 Patient modifié : {Nom} {Prenom}", notification.Patient.Id, notification.Patient.Id);
            return Task.CompletedTask;
        }
    }
}
