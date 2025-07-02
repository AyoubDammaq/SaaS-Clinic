using AutoMapper;
using MediatR;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Commands.AttacherDocument
{
    public class AttacherDocumentCommandHandler : IRequestHandler<AttacherDocumentCommand>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;
        private readonly IMapper _mapper;

        public AttacherDocumentCommandHandler(IDossierMedicalRepository repository, IMapper mapper)
        {
            _dossierMedicalRepository = repository;
            _mapper = mapper;
        }

        public async Task Handle(AttacherDocumentCommand request, CancellationToken cancellationToken)
        {
            // Types autorisés
            var allowedTypes = new[] { "pdf", "jpg", "jpeg", "png" };

            var documentType = request.document.Type?.Trim().ToLower();
            var extension = Path.GetExtension(request.document.Nom)?.ToLower().Replace(".", "");

            if (string.IsNullOrWhiteSpace(documentType) || string.IsNullOrWhiteSpace(extension))
                throw new InvalidOperationException("Type ou extension du document manquant.");

            if (!allowedTypes.Contains(documentType) || !allowedTypes.Contains(extension))
                throw new InvalidOperationException($"Le type de fichier '{documentType}' ou extension '.{extension}' n'est pas autorisé.");

            // TODO : Tu peux aussi vérifier la cohérence entre Type et extension ici.

            var dossierMedical = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(request.dossierMedicalId) ?? throw new InvalidOperationException("Dossier médical not found.");

            // Utilisation du mapping AutoMapper
            var newDocument = _mapper.Map<Document>(request.document);

            newDocument.AttacherDocumentEvent(newDocument);

            await _dossierMedicalRepository.AttacherDocumentAsync(request.dossierMedicalId, newDocument);
        }
    }
}
