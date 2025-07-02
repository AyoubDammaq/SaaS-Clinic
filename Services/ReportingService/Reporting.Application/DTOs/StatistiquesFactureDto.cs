namespace Reporting.Application.DTOs
{
    public class StatistiquesFactureDto
    {
        public int NombreTotal { get; set; }
        public int NombrePayees { get; set; }
        public int NombreImpayees { get; set; }
        public int NombrePartiellementPayees { get; set; }
        public decimal MontantTotal { get; set; }
        public decimal MontantTotalPaye { get; set; }

        public Dictionary<Guid, int> NombreParClinique { get; set; } = new();
    }
}
