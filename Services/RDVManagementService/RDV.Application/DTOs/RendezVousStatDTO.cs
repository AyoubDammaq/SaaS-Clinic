
namespace RDV.Application.DTOs
{
    public class RendezVousStatDTO
    {
        public DateTime Date { get; set; }
        public int TotalRendezVous { get; set; }
        public int Confirmes { get; set; }
        public int Annules { get; set; }
        public int EnAttente { get; set; }
    }

}
