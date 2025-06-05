using Facturation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.EventHandlers
{
    public class FacturePayedEventHandler : INotificationHandler<FacturePayed>
    {
        private readonly ILogger<FacturePayedEventHandler> _logger;

        public FacturePayedEventHandler(ILogger<FacturePayedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(FacturePayed notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("💰 Paiement effectué : {Id} pour la facture : {FactureId}", notification.Paiement.Id, notification.Paiement.FactureId);
            return Task.CompletedTask;
        }
    }
}
