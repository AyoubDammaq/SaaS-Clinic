using PatientManagementService.Models;

namespace PatientManagementService.Repositories
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
    }
}
