using DoctorManagementService.Models;

namespace DoctorManagementService.Repositories
{
    public interface IDisponibiliteRepository
    {
        Task AjouterDisponibiliteAsync(Guid medecinId, Disponibilite nouvelleDispo);
        Task SupprimerDisponibiliteAsync(Guid disponibiliteId);
        Task<List<Disponibilite>> ObtenirDisponibilitesParMedecinId(Guid medecinId);
        Task<Disponibilite> ObtenirDisponibiliteParId(Guid disponibiliteId);
        Task<List<Disponibilite>> ObtenirDisponibilites();
        Task<Disponibilite> ObtenirDisponibiliteAvecMedecin(Guid disponibiliteId);

    }
}
