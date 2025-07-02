using Facturation.Application.DTOs;
using Facturation.Domain.Events;
using Facturation.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.EventHandlers
{
    public class FactureUpdatedEventHandler : INotificationHandler<FactureUpdated>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<FactureUpdatedEventHandler> _logger;

        public FactureUpdatedEventHandler(IKafkaProducer producer, ILogger<FactureUpdatedEventHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(FactureUpdated notification, CancellationToken cancellationToken)
        {
            var facture = notification.Facture;

            var dto = new FactureUpdatedDTO
            {
                Id = facture.Id,
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                DateEmission = facture.DateEmission,
                MontantTotal = facture.MontantTotal,
                MontantPaye = facture.MontantPaye,
                Status = facture.Status
            };

            // Publier l'événement PatientAdded sur le topic Kafka
            await _producer.PublishAsync("facture-updated", dto, cancellationToken);

            _logger.LogInformation("📝 Facture modifiée : {Id}", notification.Facture.Id);
        }
    }
}
