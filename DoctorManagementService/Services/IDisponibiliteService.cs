using DoctorManagementService.Models;

namespace DoctorManagementService.Services
{
    public interface IDisponibiliteService
    {
        Task AjouterDisponibilite(Guid medecinId, Disponibilite nouvelleDispo);
        Task SupprimerDisponibilite(Guid disponibiliteId);
        Task<List<Disponibilite>> GetDisponibilitesByMedecinId(Guid medecinId);
        Task<Disponibilite> GetDisponibiliteById(Guid disponibiliteId);
        Task<List<Disponibilite>> GetDisponibilites();
        Task<Disponibilite> GetDisponibiliteWithMedecin(Guid disponibiliteId);
        Task<List<Disponibilite>> ObtenirDisponibilitesParMedecinIdEtDate(Guid medecinId, DateTime date);

    }
}
