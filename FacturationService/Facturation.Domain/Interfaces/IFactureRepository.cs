using Facturation.Domain.Entities;
using Facturation.Domain.Enums;

namespace Facturation.Domain.Interfaces
{
    public interface IFactureRepository
    {
        Task<Facture?> GetFactureByIdAsync(Guid id);
        Task<IEnumerable<Facture>> GetAllFacturesAsync();
        Task AddFactureAsync(Facture facture);
        Task UpdateFactureAsync(Facture facture);
        Task DeleteFactureAsync(Guid id);
        Task<IEnumerable<Facture>> GetAllFacturesByRangeOfDateAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Facture>> GetAllFacturesByStateAsync(FactureStatus status);
        Task<IEnumerable<Facture>> GetAllFacturesByPatientIdAsync(Guid patientId);
        Task<IEnumerable<Facture>> GetAllFacturesByClinicIdAsync(Guid clinicId);
    }
}
