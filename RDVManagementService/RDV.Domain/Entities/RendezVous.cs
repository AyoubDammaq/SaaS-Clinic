using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RDV.Domain.Enums;

namespace RDV.Domain.Entities
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
        [DataType(DataType.DateTime)]
        public DateTime DateHeure { get; set; }

        [Required]
        [EnumDataType(typeof(RDVstatus))]
        public RDVstatus Statut { get; set; }  // Ex: "Confirmé", "Annulé", "En attente"

        [MaxLength(500)]
        public string Commentaire { get; set; } = string.Empty;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateCreation { get; set; } = DateTime.Now;
    }
}
