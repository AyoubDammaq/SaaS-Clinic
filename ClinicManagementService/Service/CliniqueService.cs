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
            if (clinique == null)
                throw new ArgumentNullException(nameof(clinique), "La clinique ne peut pas être null.");

            if (string.IsNullOrWhiteSpace(clinique.Nom))
                throw new ArgumentException("Le nom de la clinique est requis.", nameof(clinique.Nom));

            if (string.IsNullOrWhiteSpace(clinique.Adresse))
                throw new ArgumentException("L'adresse de la clinique est requise.", nameof(clinique.Adresse));

            //clinique.TenantId = _tenantId;
            await _repository.AddAsync(clinique);
            return clinique;
        }

        public async Task<Clinique> ModifierCliniqueAsync(Guid id, Clinique clinique)
        {
            if (clinique == null)
                throw new ArgumentNullException(nameof(clinique), "La clinique ne peut pas être null.");

            if (id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique est requis.", nameof(id));

            clinique.Id = id;
            await _repository.UpdateAsync(clinique);
            return clinique;
        }

        public async Task<bool> SupprimerCliniqueAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique est requis.", nameof(id));

            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<Clinique> ObtenirCliniqueParIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique est requis.", nameof(id));

            var clinique = await _repository.GetByIdAsync(id);
            if (clinique == null)
                throw new KeyNotFoundException($"Aucune clinique trouvée avec l'identifiant {id}.");

            return clinique;
        }

        public async Task<List<Clinique>> ListerCliniqueAsync()
        {
            var cliniques = await _repository.GetAllAsync();
            if (cliniques == null || !cliniques.Any())
                throw new InvalidOperationException("Aucune clinique disponible.");

            return cliniques;
        }

        public async Task<IEnumerable<Clinique>> ListerCliniquesParNomAsync(string nom)
        {
            if (string.IsNullOrWhiteSpace(nom))
                throw new ArgumentException("Le nom de la clinique est requis.", nameof(nom));

            var cliniques = await _repository.GetByNameAsync(nom);
            if (cliniques == null || !cliniques.Any())
                throw new KeyNotFoundException($"Aucune clinique trouvée avec le nom '{nom}'.");

            return cliniques.Where(c => c != null).Cast<Clinique>();
        }

        public async Task<IEnumerable<Clinique>> ListerCliniquesParAdresseAsync(string adresse)
        {
            if (string.IsNullOrWhiteSpace(adresse))
                throw new ArgumentException("L'adresse de la clinique est requise.", nameof(adresse));

            var cliniques = await _repository.GetByAddressAsync(adresse);
            if (cliniques == null || !cliniques.Any())
                throw new KeyNotFoundException($"Aucune clinique trouvée avec l'adresse '{adresse}'.");

            return cliniques.Where(c => c != null).Cast<Clinique>();
        }
    }
}
