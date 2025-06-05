using Clinic.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.EventHandlers
{
    public class CliniqueUpdatedHandler : INotificationHandler<CliniqueUpdated>
    {
        private readonly ILogger<CliniqueUpdatedHandler> _logger;

        public CliniqueUpdatedHandler(ILogger<CliniqueUpdatedHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(CliniqueUpdated notification, CancellationToken cancellationToken)
        {
            var clinique = notification.Clinique;
            _logger.LogInformation("🏥✏️ Clinique mise à jour : {Id} - {Nom}", clinique.Id, clinique.Nom);

            // Tu peux ici appeler un service externe, envoyer un e-mail, notifier un autre système, etc.

            return Task.CompletedTask;
        }
    }
}
