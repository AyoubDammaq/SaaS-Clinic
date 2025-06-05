using ConsultationManagementService.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Application.EventHandlers
{
    public class MedicalDocumentRemovedEventHandler : INotificationHandler<MedicalDocumentRemoved>
    {
        private readonly ILogger<MedicalDocumentRemovedEventHandler> _logger;

        public MedicalDocumentRemovedEventHandler(ILogger<MedicalDocumentRemovedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(MedicalDocumentRemoved notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"🗑️ Document médical : {notification.DocumentMedical.Id} supprimé pour la consultation : {notification.DocumentMedical.ConsultationId}");
            return Task.CompletedTask;
        }
    }
}
