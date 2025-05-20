
using Facturation.Domain.Entities;

namespace Facturation.Domain.Interfaces
{
    public interface IPaiementRepository
    {
        Task<Paiement?> GetByFactureIdAsync(Guid factureId);
        Task AddAsync(Paiement paiement);
    }
}
