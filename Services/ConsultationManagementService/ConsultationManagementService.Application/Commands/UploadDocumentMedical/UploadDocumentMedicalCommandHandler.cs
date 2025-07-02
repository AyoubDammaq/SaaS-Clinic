using AutoMapper;
using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Commands.UploadDocumentMedical
{
    public class UploadDocumentMedicalCommandHandler : IRequestHandler<UploadDocumentMedicalCommand>
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly IMapper _mapper;

        public UploadDocumentMedicalCommandHandler(IConsultationRepository consultationRepository, IMapper mapper)
        {
            _consultationRepository = consultationRepository;
            _mapper = mapper;
        }

        public async Task Handle(UploadDocumentMedicalCommand request, CancellationToken cancellationToken)
        {
            if (request.documentMedical == null)
            {
                throw new ArgumentNullException(nameof(request.documentMedical), "Les données du document médical ne peuvent pas être nulles.");
            }

            ValidateDocumentMedicalData(request.documentMedical); // Validation des données

            // Vérifier que la consultation existe
            var consultationExists = await _consultationRepository.ExistsAsync(request.documentMedical.ConsultationId);
            if (!consultationExists)
            {
                throw new ArgumentException("La consultation associée au document n'existe pas.");
            }

            // Utilisation du mapper pour transformer le DTO en entité
            var documentEntity = _mapper.Map<DocumentMedical>(request.documentMedical);
            if (documentEntity.Id == Guid.Empty)
            {
                documentEntity.Id = Guid.NewGuid();
            }

            documentEntity.UploadMedicalDocumentEvent();

            await _consultationRepository.UploadDocumentMedicalAsync(documentEntity);
        }

        private void ValidateDocumentMedicalData(DocumentMedicalDTO document)
        {
            if (document.ConsultationId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.");
            }
            if (string.IsNullOrWhiteSpace(document.Type))
            {
                throw new ArgumentException("Le type de document ne peut pas être vide.");
            }
            if (string.IsNullOrWhiteSpace(document.FichierURL))
            {
                throw new ArgumentException("L'URL du fichier ne peut pas être vide.");
            }
        }
    }
}
