using ConsultationManagementService.DTOs;
using ConsultationManagementService.Models;
using ConsultationManagementService.Repositories;

namespace ConsultationManagementService.Services
{
    public class ConsultationService : IConsultationService 
    {
        private readonly IConsultationRepository _consultationRepository; // Changed from public to private

        public ConsultationService(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository ?? throw new ArgumentNullException(nameof(consultationRepository));
        }

        public async Task<Consultation?> GetConsultationByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.", nameof(id));
            }
            return await _consultationRepository.GetConsultationByIdAsync(id);
        }

        public async Task<IEnumerable<Consultation>> GetAllConsultationsAsync()
        {
            return await _consultationRepository.GetAllConsultationsAsync();
        }

        public async Task CreateConsultationAsync(ConsultationDTO consultation)
        {
            if (consultation == null)
            {
                throw new ArgumentNullException(nameof(consultation), "Les données de la consultation ne peuvent pas être nulles.");
            }

            ValidateConsultationData(consultation); // Validation des données

            // Transformation de ConsultationDTO en Consultation
            var consultationEntity = new Consultation
            {
                Id = consultation.Id != Guid.Empty ? consultation.Id : Guid.NewGuid(),
                PatientId = consultation.PatientId,
                MedecinId = consultation.MedecinId,
                DateConsultation = consultation.DateConsultation,
                Diagnostic = consultation.Diagnostic ?? string.Empty,
                Notes = consultation.Notes ?? string.Empty,
                Documents = consultation.Documents != null && consultation.Documents.Any()
                    ? consultation.Documents.Select(doc => new DocumentMedical
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

        public async Task UpdateConsultationAsync(ConsultationDTO consultation)
        {
            if (consultation == null)
            {
                throw new ArgumentNullException(nameof(consultation), "Les données de la consultation ne peuvent pas être nulles.");
            }
            if (consultation.Id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.", nameof(consultation));
            }

            ValidateConsultationData(consultation); // Added validation

            // Transformation du DTO vers l'entité du domaine
            var consultationEntity = new Consultation
            {
                Id = consultation.Id,
                PatientId = consultation.PatientId,
                MedecinId = consultation.MedecinId,
                DateConsultation = consultation.DateConsultation,
                Diagnostic = consultation.Diagnostic ?? string.Empty,
                Notes = consultation.Notes ?? string.Empty,
                Documents = consultation.Documents?.Select(doc => new DocumentMedical
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

        public async Task<bool> DeleteConsultationAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.", nameof(id));
            }
            return await _consultationRepository.DeleteConsultationAsync(id);
        }

        public async Task<IEnumerable<Consultation>> GetConsultationsByPatientIdAsync(Guid patientId)
        {
            if (patientId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du patient ne peut pas être vide.", nameof(patientId));
            }
            return await _consultationRepository.GetConsultationsByPatientIdAsync(patientId);
        }

        public async Task<IEnumerable<Consultation>> GetConsultationsByDoctorIdAsync(Guid doctorId)
        {
            if (doctorId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(doctorId));
            }
            return await _consultationRepository.GetConsultationsByDoctorIdAsync(doctorId);
        }

        public Task<int> GetNombreConsultationsAsync(DateTime dateDebut, DateTime dateFin)
        {
            return _consultationRepository.CountConsultationsAsync(dateDebut, dateFin);
        }

        public async Task<int> CountByMedecinIds(List<Guid> medecinIds)
        {
            if (medecinIds == null || !medecinIds.Any())
            {
                throw new ArgumentException("La liste des identifiants de médecins ne peut pas être vide.", nameof(medecinIds));
            }
            return await _consultationRepository.CountByMedecinIdsAsync(medecinIds);
        }



        public async Task<DocumentMedical?> GetDocumentMedicalByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du document médical ne peut pas être vide.", nameof(id));
            }
            return await _consultationRepository.GetDocumentMedicalByIdAsync(id);
        }

        public async Task UploadDocumentMedicalAsync(DocumentMedicalDTO documentMedical)
        {
            if (documentMedical == null)
            {
                throw new ArgumentNullException(nameof(documentMedical), "Les données du document médical ne peuvent pas être nulles.");
            }

            ValidateDocumentMedicalData(documentMedical); // Validation des données

            // Transformation de DocumentMedicalDTO en DocumentMedical
            var documentEntity = new DocumentMedical
            {
                Id = documentMedical.Id != Guid.Empty ? documentMedical.Id : Guid.NewGuid(),
                ConsultationId = documentMedical.ConsultationId,
                Type = documentMedical.Type ?? string.Empty,
                FichierURL = documentMedical.FichierURL ?? string.Empty,
                DateAjout = documentMedical.DateAjout
            };

            await _consultationRepository.UploadDocumentMedicalAsync(documentEntity);
        }

        public async Task<bool> DeleteDocumentMedicalAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du document médical ne peut pas être vide.", nameof(id));
            }
            return await _consultationRepository.DeleteDocumentMedicalAsync(id);
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
