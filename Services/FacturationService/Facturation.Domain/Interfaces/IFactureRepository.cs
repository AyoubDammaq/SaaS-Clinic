using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.ValueObjects;

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
        Task<List<Facture>> GetByFilterAsync(Guid? patientId, Guid? clinicId, FactureStatus? status);

        Task<IEnumerable<FactureStats>> GetNombreDeFactureByStatusAsync();
        Task<IEnumerable<FactureStats>> GetNombreDeFactureParCliniqueAsync();
        Task<IEnumerable<FactureStats>> GetNombreDeFacturesByStatusParCliniqueAsync();
        Task<IEnumerable<FactureStats>> GetNombreDeFacturesByStatusDansUneCliniqueAsync(Guid cliniqueId);
        Task<List<Facture>> GetFacturesParPeriode(DateTime dateDebut, DateTime dateFin);
    }
}
