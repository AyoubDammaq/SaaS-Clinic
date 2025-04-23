using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.ValueObject;

namespace PatientManagementService.Domain.Interfaces
{
    public interface IPatientRepository
    {
        // Define methods for patient repository
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByIdAsync(Guid id);
        Task AddPatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient patient);
        Task DeletePatientAsync(Guid id);
        Task<IEnumerable<Patient>> GetPatientsByNameAsync(string firstName, string lastName);
        Task<int> GetStatistiquesAsync(DateTime dateDebut, DateTime dateFin);
    }
}
