using ConsultationManagementService.Domain.Events;
using ConsultationManagementService.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Application.EventHandlers
{
    public class MedicalDocumentRemovedEventHandler : INotificationHandler<MedicalDocumentRemoved>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<MedicalDocumentRemovedEventHandler> _logger;

        public MedicalDocumentRemovedEventHandler(IKafkaProducer producer, ILogger<MedicalDocumentRemovedEventHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(MedicalDocumentRemoved notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("medicalDocument-removed", notification, cancellationToken);
            _logger.LogInformation($"🗑️ Document médical : {notification.DocumentMedical.Id} supprimé pour la consultation : {notification.DocumentMedical.ConsultationId}");
        }
    }
}
