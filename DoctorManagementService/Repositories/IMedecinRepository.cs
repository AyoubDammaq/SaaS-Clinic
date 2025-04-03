using DoctorManagementService.Models;

namespace DoctorManagementService.Repositories
{
    public interface IMedecinRepository
    {
        Task<Medecin> GetByIdAsync(Guid id);
        Task<List<Medecin>> GetAllAsync();
        Task AddAsync(Medecin medecin);
        Task UpdateAsync(Medecin medecin);
        Task DeleteAsync(Guid id);
        Task<List<Medecin>> FilterAsync(string specialite);
    }
}
