

using Facturation.Domain.Entities;
using Facturation.Domain.Enums;

namespace Facturation.Application.Interfaces
{
    public interface IPaiementService
    {
        Task<bool> PayerFactureAsync(Guid factureId, ModePaiement moyenPaiement);
        Task<byte[]> ImprimerRecuDePaiement(Paiement paiement);
        Task<Paiement?> GetPaiementByFactureIdAsync(Guid factureId);
    }
}
