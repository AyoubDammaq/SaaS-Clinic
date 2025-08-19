namespace Facturation.Domain.ValueObjects
{
    public class BillingStats
    {
        public decimal Revenue { get; set; }
        public decimal PendingAmount { get; set; }
        public decimal OverdueAmount { get; set; }
        public double PaymentRate { get; set; } // en %
    }
}
