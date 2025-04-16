using PatientManagementService.DTOs;
using PatientManagementService.Models;

namespace PatientManagementService.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient> GetPatientByIdAsync(Guid id);
        Task AddPatientAsync(PatientDTO patient);
        Task UpdatePatientAsync(PatientDTO patient);
        Task DeletePatientAsync(Guid id);
        Task<IEnumerable<Patient>> GetPatientsByNameAsync(string? name, string? lastname);
    }
}
