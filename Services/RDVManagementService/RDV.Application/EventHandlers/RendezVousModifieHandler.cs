using MediatR;
using Microsoft.Extensions.Logging;
using RDV.Domain.Events;

namespace RDV.Application.EventHandlers
{
    public class RendezVousModifieHandler : INotificationHandler<RendezVousModifie>
    {
        private readonly ILogger<RendezVousModifieHandler> _logger;

        public RendezVousModifieHandler(ILogger<RendezVousModifieHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(RendezVousModifie notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"📝 Rendez-vous modifié : {notification.RendezVous.Id}, Nouvelle date : {notification.RendezVous.DateHeure}, Commentaire : {notification.RendezVous.Commentaire}");

            // Ex: Notification du patient et médecin, mise à jour de l'agenda
            return Task.CompletedTask;
        }
    }
}
