using ClinicManagementService.Models;

namespace ClinicManagementService.Service
{
    public interface ICliniqueService
    {
        Task<Clinique> AjouterCliniqueAsync(Clinique clinique);
        Task<Clinique> ModifierCliniqueAsync(Guid id, Clinique clinique);
        Task<bool> SupprimerCliniqueAsync(Guid id);
        Task<Clinique> ObtenirCliniqueParIdAsync(Guid id);
        Task<IEnumerable<Clinique>> ListerCliniqueAsync();
    }
}
