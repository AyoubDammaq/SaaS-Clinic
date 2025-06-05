using ConsultationManagementService.Domain.Entities;

namespace ConsultationManagementService.Repositories
{
    public interface IConsultationRepository
    {
        // Méthodes pour la gestion des consultations
        Task<Consultation?> GetConsultationByIdAsync(Guid id);
        Task<IEnumerable<Consultation>> GetAllConsultationsAsync();
        Task CreateConsultationAsync(Consultation consultation);
        Task UpdateConsultationAsync(Consultation consultation);
        Task<bool> DeleteConsultationAsync(Guid id);
        Task<IEnumerable<Consultation>> GetConsultationsByPatientIdAsync(Guid patientId);
        Task<IEnumerable<Consultation>> GetConsultationsByDoctorIdAsync(Guid doctorId);
        Task<int> CountConsultationsAsync(DateTime dateDebut, DateTime dateFin);
        Task<int> CountByMedecinIdsAsync(List<Guid> medecinIds);

        // Méthodes pour la gestion des documents médicaux
        Task<DocumentMedical?> GetDocumentMedicalByIdAsync(Guid id);
        Task UploadDocumentMedicalAsync(DocumentMedical documentMedical);
        Task<bool> DeleteDocumentMedicalAsync(Guid id);
    }
}
