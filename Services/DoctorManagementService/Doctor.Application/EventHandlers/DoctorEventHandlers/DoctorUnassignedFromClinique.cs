using Doctor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DoctorEventHandlers
{
    public class DoctorUnassignedFromCliniqueHandler : INotificationHandler<DoctorUnassignedFromClinique>
    {
        private readonly ILogger<DoctorUnassignedFromCliniqueHandler> _logger;

        public DoctorUnassignedFromCliniqueHandler(ILogger<DoctorUnassignedFromCliniqueHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DoctorUnassignedFromClinique notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning("👨‍⚕️➖🏥 Médecin désabonné d'une clinique : MedecinId = {MedecinId}, CliniqueId = {CliniqueId}",
                notification.MedecinId, notification.CliniqueId);

            // Tu peux ici supprimer un lien, notifier la clinique, etc.

            return Task.CompletedTask;
        }
    }
}
