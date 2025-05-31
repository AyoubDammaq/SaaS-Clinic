using ConsultationManagementService.DTOs;
using ConsultationManagementService.Models;
using ConsultationManagementService.Repositories;
using MediatR;

namespace Consultation.Application.Commands.CreateConsultation
{
    public class CreateConsultationCommandHandler : IRequestHandler<CreateConsultationCommand>
    {
        private readonly IConsultationRepository _consultationRepository;
        public CreateConsultationCommandHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task Handle(CreateConsultationCommand request, CancellationToken cancellationToken)
        {
            if (request.consultation == null)
            {
                throw new ArgumentNullException(nameof(request.consultation), "Les données de la consultation ne peuvent pas être nulles.");
            }

            ValidateConsultationData(request.consultation); // Validation des données

            // Transformation de ConsultationDTO en Consultation
            var consultationEntity = new ConsultationManagementService.Models.Consultation
            {
                Id = request.consultation.Id != Guid.Empty ? request.consultation.Id : Guid.NewGuid(),
                PatientId = request.consultation.PatientId,
                MedecinId = request.consultation.MedecinId,
                DateConsultation = request.consultation.DateConsultation,
                Diagnostic = request.consultation.Diagnostic ?? string.Empty,
                Notes = request.consultation.Notes ?? string.Empty,
                Documents = request.consultation.Documents != null && request.consultation.Documents.Any()
                    ? request.consultation.Documents.Select(doc => new DocumentMedical
                    {
                        Id = doc.Id != Guid.Empty ? doc.Id : Guid.NewGuid(),
                        ConsultationId = doc.ConsultationId,
                        Type = doc.Type ?? string.Empty,
                        FichierURL = doc.FichierURL ?? string.Empty,
                        DateAjout = doc.DateAjout
                    }).ToList()
                    : new List<DocumentMedical>()
            };


            await _consultationRepository.CreateConsultationAsync(consultationEntity);
        }

        private void ValidateConsultationData(ConsultationDTO consultation)
        {
            if (consultation.PatientId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du patient ne peut pas être vide.");
            }
            if (consultation.MedecinId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.");
            }
            if (consultation.DateConsultation == default)
            {
                throw new ArgumentException("La date de consultation doit être spécifiée.");
            }
            if (string.IsNullOrWhiteSpace(consultation.Diagnostic))
            {
                throw new ArgumentException("Le diagnostic ne peut pas être vide.");
            }
        }
    }
}
