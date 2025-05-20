using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.Services
{
    public interface IDossierMedicalService
    {
        Task<DossierMedical?> GetDossierMedicalByPatientIdAsync(Guid patientId);
        Task AddDossierMedicalAsync(DossierMedicalDTO dossierMedical);
        Task UpdateDossierMedicalAsync(DossierMedicalDTO dossierMedical);
        Task DeleteDossierMedicalAsync(Guid dossierMedicalId);
        Task<IEnumerable<DossierMedical>> GetAllDossiersMedicalsAsync();
        Task<DossierMedical?> GetDossierMedicalByIdAsync(Guid Id);
        Task AttacherDocumentAsync(Guid dossierMedicalId, DocumentDTO document);
        Task<Document?> GetDocumentByIdAsync(Guid documentId);
        Task RemoveDocumentAsync(Guid documentId);
    }
}
