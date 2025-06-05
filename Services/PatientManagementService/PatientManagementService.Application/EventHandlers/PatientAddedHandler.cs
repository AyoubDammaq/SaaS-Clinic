using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;

namespace PatientManagementService.Application.EventHandlers
{
    public class PatientAddedHandler : INotificationHandler<PatientAdded>
    {
        private readonly ILogger<PatientAddedHandler> _logger;

        public PatientAddedHandler(ILogger<PatientAddedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(PatientAdded notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("✅ Nouveau patient ajouté : {Nom} {Prenom}", notification.Nom, notification.Prenom);
            return Task.CompletedTask;
        }
    }
}
