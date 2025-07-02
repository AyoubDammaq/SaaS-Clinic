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
            try
            {
                var document = await _dossierMedicalRepository.GetDocumentByIdAsync(request.documentId);
                if (document == null)
                    return;

                document.DetacherDocumentEvent(document);

                await _dossierMedicalRepository.RemoveDocumentAsync(request.documentId);
            }
            catch (Exception ex)
            {
                // Vous pouvez logger l'exception ici si nécessaire
                throw new ApplicationException("Erreur lors de la suppression du document.", ex);
            }
        }
    }
}
