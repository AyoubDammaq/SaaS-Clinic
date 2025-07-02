using Clinic.Domain.Events;
using Clinic.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.EventHandlers
{
    public class CliniqueDeletedHandler : INotificationHandler<CliniqueDeleted>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<CliniqueDeletedHandler> _logger;

        public CliniqueDeletedHandler(IKafkaProducer producer, ILogger<CliniqueDeletedHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(CliniqueDeleted notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("clinique-deleted", notification, cancellationToken);
            _logger.LogWarning("🏥🗑️ Clinique supprimée : {Nom}", notification.Clinique.Nom);
        }
    }
}
