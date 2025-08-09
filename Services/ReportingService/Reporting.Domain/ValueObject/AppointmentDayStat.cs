namespace Reporting.Domain.ValueObject
{
    public class AppointmentDayStat
    {
        public string Jour { get; set; } = string.Empty;
        public int Scheduled { get; set; }
        public int Pending { get; set; }
        public int Cancelled { get; set; }
    }
}
