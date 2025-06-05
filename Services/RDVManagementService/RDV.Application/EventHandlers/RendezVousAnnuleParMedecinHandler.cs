using MediatR;
using Microsoft.Extensions.Logging;
using RDV.Domain.Events;

namespace RDV.Application.EventHandlers
{
    public class RendezVousAnnuleParMedecinHandler : INotificationHandler<RendezVousAnnuleParMedecin>
    {
        private readonly ILogger<RendezVousAnnuleParMedecinHandler> _logger;

        public RendezVousAnnuleParMedecinHandler(ILogger<RendezVousAnnuleParMedecinHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(RendezVousAnnuleParMedecin notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"❌📎 Rendez-vous annulé : {notification.RendezVousId}, Raison: {notification.Raison}");

            // Ex: Envoyer une alerte email/SMS
            return Task.CompletedTask;
        }
    }
}
