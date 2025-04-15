using ConsultationManagementService.DTOs;
using ConsultationManagementService.Models;

namespace ConsultationManagementService.Services
{
    public interface IConsultationService
    {
        Task<Consultation?> GetConsultationByIdAsync(Guid id);
        Task<IEnumerable<Consultation>> GetAllConsultationsAsync();
        Task CreateConsultationAsync(ConsultationDTO consultation);
        Task UpdateConsultationAsync(ConsultationDTO consultation);
        Task<bool> DeleteConsultationAsync(Guid id);
        Task<DocumentMedical?> GetDocumentMedicalByIdAsync(Guid id);
        //Task<IEnumerable<DocumentMedicalDTO>> GetAllDocumentsMedicalsAsync();
        Task UploadDocumentMedicalAsync(DocumentMedicalDTO documentMedical);
        Task<bool> DeleteDocumentMedicalAsync(Guid id);
    }
}
