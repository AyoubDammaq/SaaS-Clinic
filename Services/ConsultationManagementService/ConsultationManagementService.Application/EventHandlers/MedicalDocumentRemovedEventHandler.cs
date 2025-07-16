using ConsultationManagementService.Application.DTOs;
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
            var doc = notification.DocumentMedical;

            var dto = new MedicalDocumentRemovedDto
            {
                DocumentId = doc.Id,
                ConsultationId = doc.ConsultationId,
                FileName = doc.FileName
            };

            await _producer.PublishAsync("medicalDocument-removed", dto, cancellationToken);
            _logger.LogInformation($"🗑️ Document médical : {dto.DocumentId} supprimé pour la consultation : {dto.ConsultationId}");
        }
    }
}
