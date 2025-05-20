
namespace Reporting.Application.DTOs
{
    public class ConsultationStatsDto
    {
        public int TotalConsultations { get; set; }
        public Dictionary<string, int> ConsultationsParJour { get; set; } = new();
    }

}
