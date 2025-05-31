using ConsultationManagementService.Repositories;
using MediatR;

namespace Consultation.Application.Commands.DeleteDocumentMedical
{
    public class DeleteDocumentMedicalCommandHandler : IRequestHandler<DeleteDocumentMedicalCommand, bool>
    {
        private readonly IConsultationRepository _consultationRepository;
        public DeleteDocumentMedicalCommandHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task<bool> Handle(DeleteDocumentMedicalCommand request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du document médical ne peut pas être vide.", nameof(request.id));
            }
            return await _consultationRepository.DeleteDocumentMedicalAsync(request.id);
        }
    }
}
