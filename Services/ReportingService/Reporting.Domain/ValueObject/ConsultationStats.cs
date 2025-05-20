
namespace Reporting.Domain.ValueObject
{
    public class ConsultationStats
    {
        public int TotalConsultations { get; set; }
        public Dictionary<string, int> ConsultationsParJour { get; set; } = new();
        public DateTime DateConsultation { get; set; }
    }

}
