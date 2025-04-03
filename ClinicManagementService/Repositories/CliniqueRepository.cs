using ClinicManagementService.Models;

namespace ClinicManagementService.Repositories
{
    public class CliniqueRepository : ICliniqueRepository
    {
        private readonly List<Clinique> cliniques;

        public CliniqueRepository()
        {
            cliniques = new List<Clinique>();
        }

        public Task<IEnumerable<Clinique>> GetAllAsync() 
        {
            return Task.FromResult<IEnumerable<Clinique>>(cliniques);
        }

        public Task<Clinique?> GetByIdAsync(Guid id) 
        { 
            return Task.FromResult(cliniques.Find(c => c.Id == id)); 
        }

        public Task AddAsync(Clinique clinique) 
        {
            cliniques.Add(clinique);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Clinique clinique) 
        {
            var existingClinique = cliniques.Find(c => c.Id == clinique.Id);
            if (existingClinique != null)
            {
                existingClinique.Nom = clinique.Nom;
                existingClinique.Adresse = clinique.Adresse;
                existingClinique.NumeroTelephone = clinique.NumeroTelephone;
                existingClinique.Email = clinique.Email;
                // Update other properties as needed
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id) 
        {
            var clinique = cliniques.Find(c => c.Id == id);
            if (clinique != null)
            {
                cliniques.Remove(clinique);
            }
            return Task.CompletedTask;
        }

        public Task<Clinique?> GetByNameAsync(string name) 
        { 
            return Task.FromResult(cliniques.Find(c => c.Nom == name)); 
        }

        public Task<Clinique?> GetByAddressAsync(string address)
        { 
            return Task.FromResult(cliniques.Find(c => c.Adresse == address));
        }
    }
}
