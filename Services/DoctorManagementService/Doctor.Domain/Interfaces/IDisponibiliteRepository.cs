using Doctor.Domain.Entities;

namespace Doctor.Domain.Interfaces
{
    public interface IDisponibiliteRepository
    {
        // 🔁 CRUD
        Task AjouterDisponibiliteAsync(Disponibilite nouvelleDispo);
        Task UpdateDisponibiliteAsync(Guid disponibiliteId, Disponibilite disponibilite);
        Task SupprimerDisponibiliteAsync(Guid disponibiliteId);

        // 🔍 Single & All
        Task<Disponibilite?> ObtenirDisponibiliteParIdAsync(Guid disponibiliteId);
        Task<List<Disponibilite>> ObtenirToutesDisponibilitesAsync();

        // 🔍 By Foreign Key
        Task<List<Disponibilite>> ObtenirDisponibilitesParMedecinIdAsync(Guid medecinId);
        Task<List<Disponibilite>> ObtenirDisponibilitesParJourAsync(Guid medecinId, DayOfWeek jour);
        Task<List<Medecin>> ObtenirMedecinsAvecDisponibilitesAsync();

        // 🔎 Availabilities Lookup
        Task<List<Medecin>> ObtenirMedecinsDisponiblesAsync(DateTime date, TimeSpan? heureDebut, TimeSpan? heureFin);
        Task<bool> EstDisponibleAsync(Guid medecinId, DateTime dateTime);

        // 🔁 Logic Helpers
        Task<bool> VerifieChevauchementAsync(Disponibilite dispo);
        Task<TimeSpan> ObtenirTempsTotalDisponibleAsync(Guid medecinId, DateTime dateDebut, DateTime dateFin);

        // Delete all availabilities for a doctor
        Task SupprimerDisponibilitesParMedecinIdAsync(Guid medecinId);

        // Get availabilities within a date range
        Task<List<Disponibilite>> ObtenirDisponibilitesDansIntervalleAsync(Guid medecinId, DateTime start, DateTime end);
    }
}
