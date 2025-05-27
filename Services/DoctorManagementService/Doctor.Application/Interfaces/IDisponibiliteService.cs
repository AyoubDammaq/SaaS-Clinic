using Doctor.Domain.Entities;

namespace Doctor.Application.Interfaces
{
    public interface IDisponibiliteService
    {
        Task AjouterDisponibilite(Disponibilite nouvelleDispo);
        Task UpdateDisponibilite(Disponibilite disponibilite);
        Task SupprimerDisponibilite(Guid disponibiliteId);
        Task<Disponibilite> GetDisponibiliteById(Guid disponibiliteId);
        Task<List<Disponibilite>> GetDisponibilites();
        Task<List<Disponibilite>> GetDisponibilitesByMedecinId(Guid medecinId);
        Task<List<Disponibilite>> GetDisponibilitesByMedecinIdAndJour(Guid medecinId, DayOfWeek jour);
        Task<List<Medecin>> GetMedecinsDisponibles(DateTime date, TimeSpan? heureDebut, TimeSpan? heureFin);
        Task<bool> IsAvailable(Guid medecinId, DateTime dateTime);
        Task<bool> CheckOverlap(Disponibilite dispo);
        Task<TimeSpan> GetTotalAvailableTime(Guid medecinId, DateTime dateDebut, DateTime dateFin);
        Task SupprimerDisponibilitesParMedecinId(Guid medecinId);
        Task<List<Disponibilite>> ObtenirDisponibilitesDansIntervalle(Guid medecinId, DateTime start, DateTime end);
    }
}
