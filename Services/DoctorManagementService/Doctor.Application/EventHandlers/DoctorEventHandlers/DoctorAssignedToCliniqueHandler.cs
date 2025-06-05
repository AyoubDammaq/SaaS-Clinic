using Doctor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DoctorEventHandlers
{
    public class DoctorAssignedToCliniqueHandler : INotificationHandler<DoctorAssignedToClinique>
    {
        private readonly ILogger<DoctorAssignedToCliniqueHandler> _logger;

        public DoctorAssignedToCliniqueHandler(ILogger<DoctorAssignedToCliniqueHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DoctorAssignedToClinique notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("👨‍⚕️➕🏥 Médecin assigné à une clinique : MedecinId = {MedecinId}, CliniqueId = {CliniqueId}",
                notification.MedecinId, notification.CliniqueId);

            // Tu peux déclencher ici d'autres actions : envoi email, sync, audit, etc.

            return Task.CompletedTask;
        }
    }
}
