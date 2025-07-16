using ConsultationManagementService.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Application.Commands.DeleteDocumentMedical
{
    public class DeleteDocumentMedicalCommandHandler : IRequestHandler<DeleteDocumentMedicalCommand, bool>
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly ILogger<DeleteDocumentMedicalCommandHandler> _logger;
        public DeleteDocumentMedicalCommandHandler(IConsultationRepository consultationRepository, ILogger<DeleteDocumentMedicalCommandHandler> logger)
        {
            _consultationRepository = consultationRepository;
            _logger = logger;
        }
        public async Task<bool> Handle(DeleteDocumentMedicalCommand request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du document médical ne peut pas être vide.", nameof(request.id));
            }

            var documentMedical = await _consultationRepository.GetDocumentMedicalByIdAsync(request.id);
            documentMedical.RemoveMedicalDocumentEvent();

            return await _consultationRepository.DeleteDocumentMedicalAsync(request.id);
        }
    }
}
