using Clinic.Domain.Events;
using Clinic.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.EventHandlers
{
    public class CliniqueUpdatedHandler : INotificationHandler<CliniqueUpdated>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<CliniqueUpdatedHandler> _logger;

        public CliniqueUpdatedHandler(IKafkaProducer producer, ILogger<CliniqueUpdatedHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(CliniqueUpdated notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("clinique-updated", notification, cancellationToken);
            var clinique = notification.Clinique;
            _logger.LogInformation("🏥✏️ Clinique mise à jour : {Id} - {Nom}", clinique.Id, clinique.Nom);
        }
    }
}
