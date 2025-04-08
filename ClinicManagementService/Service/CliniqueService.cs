using ClinicManagementService.Models;
using ClinicManagementService.Repositories;

namespace ClinicManagementService.Service
{
    public class CliniqueService : ICliniqueService
    {
        private readonly ICliniqueRepository _repository;
        private readonly Guid _tenantId;

        public CliniqueService(ICliniqueRepository repository /*, IHttpContextAccessor httpContextAccessor*/)
        {
            _repository = repository;
            // Récupération du TenantId depuis le contexte HTTP  
            /*
            _tenantId = httpContextAccessor.HttpContext?.Items["TenantId"] is Guid tenantId
                ? tenantId
                : throw new UnauthorizedAccessException("Tenant non identifié");
            */
        }

        public async Task<Clinique> AjouterCliniqueAsync(Clinique clinique)
        {
            //clinique.TenantId = _tenantId;
            await _repository.AddAsync(clinique);
            return clinique;
        }

        public async Task<Clinique> ModifierCliniqueAsync(Guid id, Clinique clinique)
        {
            clinique.Id = id;
            await _repository.UpdateAsync(clinique);
            return clinique;
        }

        public async Task<bool> SupprimerCliniqueAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<Clinique> ObtenirCliniqueParIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Clinique>> ListerCliniqueAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<Clinique>> ListerCliniquesParNomAsync(string nom)
        {
            var clinique = await _repository.GetByNameAsync(nom);
            return new List<Clinique> { clinique };
        }

        public async Task<IEnumerable<Clinique>> ListerCliniquesParAdresseAsync(string adresse)
        {
            var clinique = await _repository.GetByAddressAsync(adresse);
            return new List<Clinique> { clinique };
        }
    }
}
