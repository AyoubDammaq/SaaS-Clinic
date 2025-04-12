using ClinicManagementService.Models;

namespace ClinicManagementService.Repositories
{
    public interface ICliniqueRepository
    {
        Task<List<Clinique>> GetAllAsync();
        Task<Clinique>? GetByIdAsync(Guid id);
        Task AddAsync(Clinique clinique);
        Task UpdateAsync(Clinique clinique);
        Task DeleteAsync(Guid id);
        Task<List<Clinique?>> GetByNameAsync(string name);
        Task<List<Clinique?>> GetByAddressAsync(string address);
    }
}
