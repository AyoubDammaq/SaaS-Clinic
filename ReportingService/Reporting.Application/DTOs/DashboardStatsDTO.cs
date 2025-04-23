

namespace Reporting.Application.DTOs
{
    public class DashboardStatsDTO
    {
        public int ConsultationsJour { get; set; }
        public int NouveauxPatients { get; set; }
        public int NombreFactures { get; set; }
        public decimal TotalFacturesPayees { get; set; }
        public decimal TotalFacturesImpayees { get; set; }
        public decimal PaiementsPayes { get; set; }
        public decimal PaiementsImpayes { get; set; }
        public decimal PaiementsEnAttente { get; set; }
    }
}
