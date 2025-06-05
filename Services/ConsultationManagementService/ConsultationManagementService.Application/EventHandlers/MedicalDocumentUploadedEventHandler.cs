using ConsultationManagementService.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Application.EventHandlers
{
    public class MedicalDocumentUploadedEventHandler : INotificationHandler<MedicalDocumentUploaded>
    {
        private readonly ILogger<MedicalDocumentUploadedEventHandler> _logger;

        public MedicalDocumentUploadedEventHandler(ILogger<MedicalDocumentUploadedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(MedicalDocumentUploaded notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"📄 Document médical : {notification.DocumentMedical.Id} téléchargé pour la consultation : {notification.DocumentMedical.ConsultationId}");
            return Task.CompletedTask;
        }
    }
}
