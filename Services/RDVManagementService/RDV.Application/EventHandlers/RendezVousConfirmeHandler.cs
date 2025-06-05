using MediatR;
using Microsoft.Extensions.Logging;
using RDV.Domain.Events;

namespace RDV.Application.EventHandlers
{
    public class RendezVousConfirmeHandler : INotificationHandler<RendezVousConfirme>
    {
        private readonly ILogger<RendezVousConfirmeHandler> _logger;

        public RendezVousConfirmeHandler(ILogger<RendezVousConfirmeHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(RendezVousConfirme notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"✅ Rendez-vous confirmé : {notification.RendezVousId}");

            // Ex: Mise à jour d’un calendrier partagé, envoi de mail de confirmation, etc.
            return Task.CompletedTask;
        }
    }
}
