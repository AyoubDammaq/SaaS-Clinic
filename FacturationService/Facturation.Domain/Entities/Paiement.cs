using Facturation.Domain.Enums;


namespace Facturation.Domain.Entities
{
    public class Paiement
    {
        public int Id { get; set; }
        public double Montznt { get; set; }
        public DateTime DatePaiement { get; set; }
        public ModePaiement Mode { get; set; }
        public Guid FactureId { get; set; }
    }
}
