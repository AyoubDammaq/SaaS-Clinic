using MediatR;
using Microsoft.Extensions.Logging;
using RDV.Domain.Events;

namespace RDV.Application.EventHandlers
{
    internal class RendezVousAnnuleHandler : INotificationHandler<RendezVousAnnule>
    {
        private readonly ILogger<RendezVousAnnuleHandler> _logger;

        public RendezVousAnnuleHandler(ILogger<RendezVousAnnuleHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(RendezVousAnnule notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"❌ Rendez-vous annulé : {notification.RendezVousId} par le patient");

            // Ex: Envoyer une alerte email/SMS
            return Task.CompletedTask;
        }
    }
}
