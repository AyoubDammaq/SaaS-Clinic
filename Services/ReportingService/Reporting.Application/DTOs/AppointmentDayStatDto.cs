namespace Reporting.Application.DTOs
{
    public class AppointmentDayStatDto
    {
        public string Jour { get; set; } = string.Empty;
        public int Scheduled { get; set; }
        public int Pending { get; set; }
        public int Cancelled { get; set; }
    }
}
