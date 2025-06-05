using MediatR;
using Microsoft.Extensions.Logging;
using RDV.Domain.Events;

namespace RDV.Application.EventHandlers
{
    public class RendezVousCreeHandler : INotificationHandler<RendezVousCree>
    {
        private readonly ILogger<RendezVousCreeHandler> _logger;

        public RendezVousCreeHandler(ILogger<RendezVousCreeHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(RendezVousCree notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"🆕 Rendez-vous créé : {notification.RendezVous.Id}, Médecin: {notification.RendezVous.MedecinId}, Patient: {notification.RendezVous.PatientId}, Date: {notification.RendezVous.DateHeure}");

            // Ex: Envoyer une notification, publier sur un bus, etc.
            return Task.CompletedTask;
        }
    }
}
