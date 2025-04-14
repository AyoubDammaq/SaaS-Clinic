using PatientManagementService.DTOs;
using PatientManagementService.Models;

namespace PatientManagementService.Repositories
{
    public interface IPatientRepository
    {
        // Define methods for patient repository
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient> GetPatientByIdAsync(Guid id);
        Task AddPatientAsync(PatientDTO patient);
        Task UpdatePatientAsync(PatientDTO patient);
        Task DeletePatientAsync(Guid id);
        Task<IEnumerable<Patient>> GetPatientsByNameAsync(string name, string lastname);
    }
}
