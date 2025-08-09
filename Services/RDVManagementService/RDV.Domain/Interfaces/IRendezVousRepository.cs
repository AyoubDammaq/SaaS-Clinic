using RDV.Domain.Entities;
using RDV.Domain.Enums;

namespace RDV.Domain.Interfaces
{
    public interface IRendezVousRepository
    {
        Task<IEnumerable<RendezVous>> GetAllRendezVousAsync();
        Task<RendezVous> GetRendezVousByIdAsync(Guid id);
        Task CreateRendezVousAsync(RendezVous rendezVous);
        Task UpdateRendezVousAsync(Guid id, RendezVous rendezVous);
        Task<bool> AnnulerRendezVousAsync(Guid id);
        Task<IEnumerable<RendezVous>> GetRendezVousByPatientIdAsync(Guid patientId);
        Task<IEnumerable<RendezVous>> GetRendezVousByMedecinIdAsync(Guid medecinId);
        Task<IEnumerable<RendezVous>> GetRendezVousByDateAsync(DateTime date);
        Task<IEnumerable<RendezVous>> GetRendezVousByStatutAsync(RDVstatus statut);
        Task<IEnumerable<RendezVous>> GetRendezVousByPeriod(DateTime start, DateTime end);
        Task<IEnumerable<RendezVous>> GetRendezVousByMedecinAndDateAsync(Guid medecinId, DateTime date);
        Task<int> CountPendingRDVByDoctorAsync(Guid medecinId);
        Task<int>CountPendingRDVByClinicAsync(Guid clinicId);
        Task<int> CountByMedecinsIdsAndDateAsync(List<Guid> medecinsIds, DateTime date);
        Task<int> CountDistinctPatientsByMedecinIdsAsync(List<Guid> medecinIds);
        Task<int> CountByMedecinIdsAsync(List<Guid> medecinIds);

        Task ConfirmerRendezVousParMedecin(Guid rendezVousId);
        Task AnnulerRendezVousParMedecin(Guid rendezVousId, string justification);

        Task<bool> ExisteRendezVousPourMedecinEtDate(Guid medecinId, DateTime dateHeure);
    }
}
