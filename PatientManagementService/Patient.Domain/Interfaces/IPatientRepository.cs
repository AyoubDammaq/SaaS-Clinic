﻿using PatientManagementService.Models;

namespace PatientManagementService.Repositories
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
    }
}
