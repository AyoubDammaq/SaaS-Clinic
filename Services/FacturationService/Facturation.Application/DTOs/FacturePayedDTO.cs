using Facturation.Domain.Enums;

namespace Facturation.Application.DTOs
{
    public class FacturePayedDTO
    {
        public Guid PaiementId { get; set; }
        public decimal Montant { get; set; }
        public DateTime DatePaiement { get; set; }
        public ModePaiement Mode { get; set; }
        public Guid FactureId { get; set; }
    }
}
