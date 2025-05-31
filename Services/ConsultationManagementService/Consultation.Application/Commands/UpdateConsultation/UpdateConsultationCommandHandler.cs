using ConsultationManagementService.DTOs;
using ConsultationManagementService.Models;
using ConsultationManagementService.Repositories;
using MediatR;

namespace Consultation.Application.Commands.UpdateConsultation
{
    public class UpdateConsultationCommandHandler : IRequestHandler<UpdateConsultationCommand>
    {
        private readonly IConsultationRepository _consultationRepository;
        public UpdateConsultationCommandHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task Handle(UpdateConsultationCommand request, CancellationToken cancellationToken)
        {
            if (request.consultation == null)
            {
                throw new ArgumentNullException(nameof(request.consultation), "Les données de la consultation ne peuvent pas être nulles.");
            }
            if (request.consultation.Id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.", nameof(request.consultation));
            }

            ValidateConsultationData(request.consultation); // Added validation

            // Transformation du DTO vers l'entité du domaine
            var consultationEntity = new ConsultationManagementService.Models.Consultation
            {
                Id = request.consultation.Id,
                PatientId = request.consultation.PatientId,
                MedecinId = request.consultation.MedecinId,
                DateConsultation = request.consultation.DateConsultation,
                Diagnostic = request.consultation.Diagnostic ?? string.Empty,
                Notes = request.consultation.Notes ?? string.Empty,
                Documents = request.consultation.Documents?.Select(doc => new DocumentMedical
                {
                    Id = doc.Id,
                    ConsultationId = doc.ConsultationId,
                    Type = doc.Type ?? string.Empty,
                    FichierURL = doc.FichierURL ?? string.Empty,
                    DateAjout = doc.DateAjout
                }).ToList() ?? new List<DocumentMedical>()
            };

            await _consultationRepository.UpdateConsultationAsync(consultationEntity);
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
