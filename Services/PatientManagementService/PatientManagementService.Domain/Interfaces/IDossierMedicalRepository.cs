using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Domain.Interfaces
{
    public interface IDossierMedicalRepository
    {
        Task<DossierMedical?> GetDossierMedicalByPatientIdAsync(Guid patientId);
        Task AddDossierMedicalAsync(DossierMedical dossierMedical);
        Task UpdateDossierMedicalAsync(DossierMedical dossierMedical);
        Task DeleteDossierMedicalAsync(Guid dossierMedicalId);
        Task<IEnumerable<DossierMedical>> GetAllDossiersMedicalsAsync();
        Task<DossierMedical?> GetDossierMedicalByIdAsync(Guid Id);
        Task AttacherDocumentAsync(Guid dossierMedicalId, Document document);
        Task<Document?> GetDocumentByIdAsync(Guid documentId);
        Task RemoveDocumentAsync(Guid documentId);
    }
}
