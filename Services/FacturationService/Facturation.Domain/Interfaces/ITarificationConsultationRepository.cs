using Facturation.Domain.Entities;

namespace Facturation.Domain.Interfaces
{
    public interface ITarificationConsultationRepository
    {
        Task<decimal?> GetMontantAsync(Guid clinicId, int typeConsultation);
        Task<IEnumerable<TarifConsultation>> GetAllAsync();
        Task<TarifConsultation?> GetByIdAsync(Guid id);
        Task<IEnumerable<TarifConsultation>> GetByClinicIdAsync(Guid cliniqueId);
        Task AddAsync(TarifConsultation tarif);
        Task UpdateAsync(TarifConsultation tarif);
        Task DeleteAsync(Guid id);
    }
}
