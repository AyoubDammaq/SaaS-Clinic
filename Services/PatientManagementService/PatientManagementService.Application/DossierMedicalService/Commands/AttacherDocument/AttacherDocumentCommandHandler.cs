using MediatR;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Commands.AttacherDocument
{
    public class AttacherDocumentCommandHandler : IRequestHandler<AttacherDocumentCommand>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;

        public AttacherDocumentCommandHandler(IDossierMedicalRepository repository)
        {
            _dossierMedicalRepository = repository;
        }

        public async Task Handle(AttacherDocumentCommand request, CancellationToken cancellationToken)
        {
            var dossierMedical = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(request.dossierMedicalId) ?? throw new InvalidOperationException("Dossier médical not found.");
            var newDucument = new Document
            {
                Nom = request.document.Nom,
                Url = request.document.Url,
                Type = request.document.Type
            };
            await _dossierMedicalRepository.AttacherDocumentAsync(request.dossierMedicalId, newDucument);
        }
    }
}
