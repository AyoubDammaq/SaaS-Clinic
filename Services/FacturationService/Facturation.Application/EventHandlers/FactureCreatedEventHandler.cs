using Facturation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.EventHandlers
{
    public class FactureCreatedEventHandler : INotificationHandler<FactureCreated>
    {
        private readonly ILogger<FactureCreatedEventHandler> _logger;

        public FactureCreatedEventHandler(ILogger<FactureCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(FactureCreated notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("✅ Nouvelle facture créée : {Id} de montant : {MontantTotal} $", notification.Facture.Id, notification.Facture.MontantTotal);
            return Task.CompletedTask;
        }
    }
}
