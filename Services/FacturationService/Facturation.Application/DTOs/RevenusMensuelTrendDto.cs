namespace Facturation.Application.DTOs
{
    public class RevenusMensuelTrendDto
    {
        public decimal Current { get; set; }
        public decimal Previous { get; set; }
        public double PercentageChange { get; set; }
        public bool IsPositive { get; set; }
    }
}
