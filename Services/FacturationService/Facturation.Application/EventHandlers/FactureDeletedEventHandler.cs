using Facturation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.EventHandlers
{
    public class FactureDeletedEventHandler : INotificationHandler<FactureDeleted>
    {
        private readonly ILogger<FactureDeletedEventHandler> _logger;

        public FactureDeletedEventHandler(ILogger<FactureDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(FactureDeleted notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🗑️ Facture supprimée : {FactureId}", notification.FactureId);
            return Task.CompletedTask;
        }
    }
}
