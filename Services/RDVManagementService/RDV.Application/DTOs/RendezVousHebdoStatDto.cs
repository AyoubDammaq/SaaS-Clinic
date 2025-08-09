namespace RDV.Application.DTOs
{
    public class RendezVousHebdoStatDto
    {
        public string Jour { get; set; } // Ex: "Lundi"
        public int Scheduled { get; set; }
        public int Pending { get; set; }
        public int Cancelled { get; set; }
    }

}
