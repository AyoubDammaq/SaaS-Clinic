using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Events;

namespace PatientManagementService.Application.EventHandlers
{
    public class PatientDeletedHandler : INotificationHandler<PatientDeleted>
    {
        private readonly ILogger<PatientDeletedHandler> _logger;

        public PatientDeletedHandler(ILogger<PatientDeletedHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(PatientDeleted notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🗑️ Patient supprimé : {PatientId}", notification.PatientId);
            return Task.CompletedTask;
        }
    }
}
