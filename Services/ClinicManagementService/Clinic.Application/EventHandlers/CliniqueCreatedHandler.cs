using Clinic.Domain.Events;
using Clinic.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.EventHandlers
{
    public class CliniqueCreatedHandler : INotificationHandler<CliniqueCreated>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<CliniqueCreatedHandler> _logger;

        public CliniqueCreatedHandler(IKafkaProducer producer, ILogger<CliniqueCreatedHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(CliniqueCreated notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("clinique-created", notification, cancellationToken);
            _logger.LogInformation("🏥➕ Nouvelle clinique créée : {Nom}", notification.Clinique.Nom);
        }
    }
}
