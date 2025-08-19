namespace Facturation.Application.DTOs
{
    public class BillingStatsDto
    {
        public decimal Revenue { get; set; }
        public decimal PendingAmount { get; set; }
        public decimal OverdueAmount { get; set; }
        public double PaymentRate { get; set; } // en %
    }
}
