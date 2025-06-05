using MediatR;
using Microsoft.Extensions.Logging;
using Doctor.Domain.Events;

namespace Doctor.Application.EventHandlers.DoctorEventHandlers
{
    public class DoctorUpdatedHandler : INotificationHandler<DoctorUpdated>
    {
        private readonly ILogger<DoctorUpdatedHandler> _logger;
        public DoctorUpdatedHandler(ILogger<DoctorUpdatedHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(DoctorUpdated notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 Doctor updated: Dr.{Nom} {Prenom}", notification.Medecin.Nom, notification.Medecin.Prenom);
            return Task.CompletedTask;
        }
    }
}
