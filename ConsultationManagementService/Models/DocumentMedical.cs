using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConsultationManagementService.Models
{
    public class DocumentMedical
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ConsultationId { get; set; } // Lien avec la Consultation

        [Required]
        public string Type { get; set; }  // Ex: "Ordonnance", "Radio", "Analyse", etc.

        [Required]
        public string FichierURL { get; set; } // Lien vers le fichier stocké

        public DateTime DateAjout { get; set; } = DateTime.Now;
    }
}