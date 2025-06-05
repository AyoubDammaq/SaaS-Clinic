using Facturation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.EventHandlers
{
    public class FactureUpdatedEventHandler : INotificationHandler<FactureUpdated>
    {
        private readonly ILogger<FactureUpdatedEventHandler> _logger;

        public FactureUpdatedEventHandler(ILogger<FactureUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(FactureUpdated notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 Facture modifiée : {Id}", notification.Facture.Id);
            return Task.CompletedTask;
        }
    }
}
