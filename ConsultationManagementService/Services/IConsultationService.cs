using ConsultationManagementService.DTOs;
using ConsultationManagementService.Models;

namespace ConsultationManagementService.Services
{
    public interface IConsultationService
    {
        // Méthodes pour la gestion des consultations
        Task<Consultation?> GetConsultationByIdAsync(Guid id);
        Task<IEnumerable<Consultation>> GetAllConsultationsAsync();
        Task CreateConsultationAsync(ConsultationDTO consultation);
        Task UpdateConsultationAsync(ConsultationDTO consultation);
        Task<bool> DeleteConsultationAsync(Guid id);

        // Méthodes pour la gestion des documents médicaux
        Task<DocumentMedical?> GetDocumentMedicalByIdAsync(Guid id);
        Task UploadDocumentMedicalAsync(DocumentMedicalDTO documentMedical);
        Task<bool> DeleteDocumentMedicalAsync(Guid id);
    }
}
