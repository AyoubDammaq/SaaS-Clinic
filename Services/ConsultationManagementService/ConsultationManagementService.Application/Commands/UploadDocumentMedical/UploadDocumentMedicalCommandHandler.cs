using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Commands.UploadDocumentMedical
{
    public class UploadDocumentMedicalCommandHandler : IRequestHandler<UploadDocumentMedicalCommand>
    {
        private readonly IConsultationRepository _consultationRepository;
        public UploadDocumentMedicalCommandHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task Handle(UploadDocumentMedicalCommand request, CancellationToken cancellationToken)
        {
            if (request.documentMedical == null)
            {
                throw new ArgumentNullException(nameof(request.documentMedical), "Les données du document médical ne peuvent pas être nulles.");
            }

            ValidateDocumentMedicalData(request.documentMedical); // Validation des données

            // Transformation de DocumentMedicalDTO en DocumentMedical
            var documentEntity = new DocumentMedical
            {
                Id = request.documentMedical.Id != Guid.Empty ? request.documentMedical.Id : Guid.NewGuid(),
                ConsultationId = request.documentMedical.ConsultationId,
                Type = request.documentMedical.Type ?? string.Empty,
                FichierURL = request.documentMedical.FichierURL ?? string.Empty,
                DateAjout = request.documentMedical.DateAjout
            };

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
