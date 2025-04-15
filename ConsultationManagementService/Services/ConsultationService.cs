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

            ValidateConsultationData(consultation); // Added validation
            await _consultationRepository.CreateConsultationAsync(consultation);
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
            await _consultationRepository.UpdateConsultationAsync(consultation);
        }

        public async Task<bool> DeleteConsultationAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.", nameof(id));
            }
            return await _consultationRepository.DeleteConsultationAsync(id);
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

            ValidateDocumentMedicalData(documentMedical); // Added validation
            await _consultationRepository.UploadDocumentMedicalAsync(documentMedical);
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
