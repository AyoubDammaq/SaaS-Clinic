using ConsultationManagementService.Domain.Events;
using ConsultationManagementService.Domain.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Application.EventHandlers
{
    public class MedicalDocumentUploadedEventHandler : INotificationHandler<MedicalDocumentUploaded>
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<MedicalDocumentUploadedEventHandler> _logger;

        public MedicalDocumentUploadedEventHandler(IKafkaProducer producer, ILogger<MedicalDocumentUploadedEventHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task Handle(MedicalDocumentUploaded notification, CancellationToken cancellationToken)
        {
            await _producer.PublishAsync("medicalDocument-uploaded", notification, cancellationToken);
            _logger.LogInformation($"📄 Document médical : {notification.DocumentMedical.Id} téléchargé pour la consultation : {notification.DocumentMedical.ConsultationId}");
        }
    }
}
