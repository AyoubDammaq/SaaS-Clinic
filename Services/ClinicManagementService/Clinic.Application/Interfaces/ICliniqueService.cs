using Clinic.Application.DTOs;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.Interfaces
{
    public interface ICliniqueService
    {
        // CRUD operations
        Task<Clinique> AjouterCliniqueAsync(CliniqueDto clinique);
        Task<Clinique> ModifierCliniqueAsync(Guid id, Clinique clinique);
        Task<bool> SupprimerCliniqueAsync(Guid id);
        Task<Clinique> ObtenirCliniqueParIdAsync(Guid id);
        Task<List<Clinique>> ListerCliniqueAsync();

        // Recherche des cliniques
        Task<IEnumerable<Clinique>> ListerCliniquesParNomAsync(string nom);
        Task<IEnumerable<Clinique>> ListerCliniquesParAdresseAsync(string adresse);
        Task<IEnumerable<Clinique>> ListerCliniquesParTypeAsync(TypeClinique type);
        Task<IEnumerable<Clinique>> ListerCliniquesParStatutAsync(StatutClinique statut);

        //Statistques des cliniques
        Task<int> GetNombreCliniques();
        Task<int> GetNombreNouvellesCliniquesDuMois();
        Task<IEnumerable<StatistiqueDTO>> GetNombreNouvellesCliniquesParMois();
        Task<StatistiqueCliniqueDTO> GetStatistiquesDesCliniquesAsync(Guid cliniqueId);
    }
}
