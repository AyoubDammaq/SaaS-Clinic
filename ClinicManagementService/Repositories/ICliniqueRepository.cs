using ClinicManagementService.Models;

namespace ClinicManagementService.Repositories
{
    public interface ICliniqueRepository
    {
        Task<IEnumerable<Clinique>> GetAllAsync();
        Task<Clinique>? GetByIdAsync(Guid id);
        Task AddAsync(Clinique clinique);
        Task UpdateAsync(Clinique clinique);
        Task DeleteAsync(Guid id);
        Task<Clinique?> GetByNameAsync(string name);
        Task<Clinique?> GetByAddressAsync(string address);
    }
}
