using Clinic.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.EventHandlers
{
    public class CliniqueCreatedHandler : INotificationHandler<CliniqueCreated>
    {
        private readonly ILogger<CliniqueCreatedHandler> _logger;

        public CliniqueCreatedHandler(ILogger<CliniqueCreatedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(CliniqueCreated notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🏥➕ Nouvelle clinique créée : {Nom}", notification.Clinique.Nom);
            return Task.CompletedTask;
        }
    }
}
