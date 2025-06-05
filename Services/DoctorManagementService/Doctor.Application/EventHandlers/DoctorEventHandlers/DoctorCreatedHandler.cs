using Doctor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DoctorEventHandlers
{
    public class DoctorCreatedHandler : INotificationHandler<DoctorCreated>
    {
        private readonly ILogger<DoctorCreatedHandler> _logger;
        public DoctorCreatedHandler(ILogger<DoctorCreatedHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(DoctorCreated notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("✅ Doctor created: Dr.{Nom} {Prenom}", notification.Medecin.Nom, notification.Medecin.Prenom);
            return Task.CompletedTask;
        }
    }
}
