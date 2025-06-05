using Clinic.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.EventHandlers
{
    public class CliniqueDeletedHandler : INotificationHandler<CliniqueDeleted>
    {
        private readonly ILogger<CliniqueDeletedHandler> _logger;

        public CliniqueDeletedHandler(ILogger<CliniqueDeletedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(CliniqueDeleted notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning("🏥🗑️ Clinique supprimée : {Nom}", notification.Clinique.Nom);
            return Task.CompletedTask;
        }
    }
}
