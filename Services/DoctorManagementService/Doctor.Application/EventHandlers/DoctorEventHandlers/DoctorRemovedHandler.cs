using MediatR;
using Microsoft.Extensions.Logging;
using Doctor.Domain.Events;

namespace Doctor.Application.EventHandlers.DoctorEventHandlers
{
    public class DoctorRemovedHandler : INotificationHandler<DoctorRemoved>
    {
        private readonly ILogger<DoctorRemovedHandler> _logger;
        public DoctorRemovedHandler(ILogger<DoctorRemovedHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(DoctorRemoved notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🗑️ Doctor removed: Dr.{Nom} {Prenom}", notification.Medecin.Nom, notification.Medecin.Prenom);
            return Task.CompletedTask;
        }
    }
}
