using Doctor.Domain.Events.DisponibilityEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.EventHandlers.DisponibiliteEventHandlers
{
    public class DisponibilitesSupprimeesParMedecinHandler : INotificationHandler<DisponibilitesSupprimeesParMedecin>
    {
        private readonly ILogger<DisponibilitesSupprimeesParMedecinHandler> _logger;

        public DisponibilitesSupprimeesParMedecinHandler(ILogger<DisponibilitesSupprimeesParMedecinHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DisponibilitesSupprimeesParMedecin notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning("❌🗑️🕒 Toutes les disponibilités du médecin {MedecinId} ont été supprimées.", notification.MedecinId);
            return Task.CompletedTask;
        }
    }
}
