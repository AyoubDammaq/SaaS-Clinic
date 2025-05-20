using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.Interfaces;

namespace Clinic.Application.Services
{
    public class CliniqueService : ICliniqueService
    {
        private readonly ICliniqueRepository _repository;

        public CliniqueService(ICliniqueRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Le dépôt de clinique ne peut pas être null.");
        }

        // CRUD operations
        public async Task<Clinique> AjouterCliniqueAsync(CliniqueDto cliniqueDto)
        {
            if (cliniqueDto == null)
                throw new ArgumentNullException(nameof(cliniqueDto), "La clinique ne peut pas être null.");

            if (string.IsNullOrWhiteSpace(cliniqueDto.Nom))
                throw new ArgumentException("Le nom de la clinique est requis.", nameof(cliniqueDto.Nom));

            if (string.IsNullOrWhiteSpace(cliniqueDto.Adresse))
                throw new ArgumentException("L'adresse de la clinique est requise.", nameof(cliniqueDto.Adresse));

            var clinique = new Clinique
            {
                Nom = cliniqueDto.Nom,
                Adresse = cliniqueDto.Adresse,
                NumeroTelephone = cliniqueDto.NumeroTelephone,
                Email = cliniqueDto.Email,
                SiteWeb = cliniqueDto.SiteWeb,
                Description = cliniqueDto.Description,
                TypeClinique = cliniqueDto.TypeClinique,
                Statut = cliniqueDto.Statut,
            };
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
            return cliniques ?? new List<Clinique>();
        }

        // Rechercher des cliniques par nom ou adresse
        public async Task<IEnumerable<Clinique>> ListerCliniquesParNomAsync(string nom)
        {
            if (string.IsNullOrWhiteSpace(nom))
                throw new ArgumentException("Le nom de la clinique est requis.", nameof(nom));

            var cliniques = await _repository.GetByNameAsync(nom);
            return cliniques?.Where(c => c != null) ?? Enumerable.Empty<Clinique>();
        }

        public async Task<IEnumerable<Clinique>> ListerCliniquesParAdresseAsync(string adresse)
        {
            if (string.IsNullOrWhiteSpace(adresse))
                throw new ArgumentException("L'adresse de la clinique est requise.", nameof(adresse));

            var cliniques = await _repository.GetByAddressAsync(adresse);
            return cliniques?.Where(c => c != null) ?? Enumerable.Empty<Clinique>();
        }

        public async Task<IEnumerable<Clinique>> ListerCliniquesParTypeAsync(TypeClinique type)
        {
            var cliniques = await _repository.GetByTypeAsync(type);
            return cliniques?.Where(c => c != null) ?? Enumerable.Empty<Clinique>();
        }

        public async Task<IEnumerable<Clinique>> ListerCliniquesParStatutAsync(StatutClinique statut)
        {
            var cliniques = await _repository.GetByStatusAsync(statut);
            return cliniques?.Where(c => c != null) ?? Enumerable.Empty<Clinique>();
        }


        // Statistiques des cliniques
        public async Task<int> GetNombreCliniques()
        {
            var nombre = await _repository.GetNombreCliniquesAsync();
            if (nombre < 0)
                throw new InvalidOperationException("Le nombre de cliniques ne peut pas être négatif.");
            return nombre;
        }

        public async Task<int> GetNombreNouvellesCliniquesDuMois()
        {
            var nombre = await _repository.GetNombreNouvellesCliniquesDuMoisAsync();
            if (nombre < 0)
                throw new InvalidOperationException("Le nombre de nouvelles cliniques ne peut pas être négatif.");
            return nombre;
        }

        public async Task<IEnumerable<StatistiqueDTO>> GetNombreNouvellesCliniquesParMois()
        {
            var statistiques = await _repository.GetNombreNouvellesCliniquesParMoisAsync();
            if (statistiques == null || !statistiques.Any())
                throw new InvalidOperationException("Aucune statistique trouvée.");
            return statistiques.Select(s => new StatistiqueDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            }).ToList();
        }

        public async Task<StatistiqueCliniqueDTO> GetStatistiquesDesCliniquesAsync(Guid cliniqueId)
        {
            if (cliniqueId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique est requis.", nameof(cliniqueId));
            var statistique = await _repository.GetStatistiquesDesCliniquesAsync(cliniqueId);
            if (statistique == null)
                throw new KeyNotFoundException($"Aucune statistique trouvée pour la clinique avec l'identifiant {cliniqueId}.");
            var dto = new StatistiqueCliniqueDTO
            {
                CliniqueId = statistique.CliniqueId,
                Nom = statistique.Nom,
                NombreMedecins = statistique.NombreMedecins,
                NombreConsultations = statistique.NombreConsultations,
                NombreRendezVous = statistique.NombreRendezVous,
                NombrePatients = statistique.NombrePatients
            };
            return dto;

        }
    }
}
