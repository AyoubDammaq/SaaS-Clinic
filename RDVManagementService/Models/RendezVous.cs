using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RDVManagementService.Models
{
    public class RendezVous
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }  // Association avec le microservice Patient

        [Required]
        public Guid MedecinId { get; set; }  // Association avec le microservice Médecin

        [Required]
        public DateTime DateHeure { get; set; }

        [Required]
        public RDVstatus Statut { get; set; }  // Ex: "Confirmé", "Annulé", "En attente"

        public string Motif { get; set; }
    }
}
