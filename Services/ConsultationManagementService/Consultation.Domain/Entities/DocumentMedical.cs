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
        [ForeignKey("Consultation")]
        public Guid ConsultationId { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;  // Ex: "Ordonnance", "Radio", "Analyse", etc.

        [Required]
        [Url] 
        public string FichierURL { get; set; } = string.Empty;

        [Required]
        public DateTime DateAjout { get; set; } = DateTime.Now;

        // Navigation property
        public virtual Consultation? Consultation { get; set; }
    }
}