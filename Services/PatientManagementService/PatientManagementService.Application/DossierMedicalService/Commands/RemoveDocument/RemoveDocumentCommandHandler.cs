using MediatR;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Commands.RemoveDocument
{
    public class RemoveDocumentCommandHandler : IRequestHandler<RemoveDocumentCommand>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;
        public RemoveDocumentCommandHandler(IDossierMedicalRepository dossierMedicalRepository)
        {
            _dossierMedicalRepository = dossierMedicalRepository;
        }
        public async Task Handle(RemoveDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _dossierMedicalRepository.GetDocumentByIdAsync(request.documentId);
            if (document == null)
                throw new Exception("Document not found");

            document.DetacherDocumentEvent(document);

            await _dossierMedicalRepository.RemoveDocumentAsync(request.documentId);
        }
    }
}
