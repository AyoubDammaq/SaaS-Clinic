using Clinic.Domain.Entities;
using Clinic.Domain.ValueObject;

namespace Clinic.Domain.Interfaces
{
    public interface ICliniqueRepository
    {
        /// CRUD operations
        Task<List<Clinique>> GetAllAsync();
        Task<Clinique>? GetByIdAsync(Guid id);
        Task AddAsync(Clinique clinique);
        Task UpdateAsync(Clinique clinique);
        Task DeleteAsync(Guid id);

        /// Search operations
        Task<List<Clinique?>> GetByNameAsync(string name);
        Task<List<Clinique?>> GetByAddressAsync(string address);

        /// Statistics operations
        Task<int> GetNombreCliniquesAsync();
        Task<int> GetNombreNouvellesCliniquesDuMoisAsync();
        Task<IEnumerable<Statistique>> GetNombreNouvellesCliniquesParMoisAsync();
        Task<StatistiqueClinique> GetStatistiquesDesCliniquesAsync(Guid cliniqueId);
    }
}
