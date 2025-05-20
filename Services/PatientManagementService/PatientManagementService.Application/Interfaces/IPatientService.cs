using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByIdAsync(Guid id);
        Task<bool> AddPatientAsync(PatientDTO patient);
        Task<bool> UpdatePatientAsync(PatientDTO patient);
        Task<bool> DeletePatientAsync(Guid id);
        Task<IEnumerable<Patient>> GetPatientsByNameAsync(string? name, string? lastname);
        Task<int> GetStatistiquesAsync(DateTime dateDebut, DateTime dateFin);
    }
}
