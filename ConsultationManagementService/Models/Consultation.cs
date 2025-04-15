using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConsultationManagementService.Models
{
    public class Consultation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }  

        [Required]
        public Guid MedecinId { get; set; }  

        [Required]
        public DateTime DateConsultation { get; set; }

        [Required]
        [StringLength(500)] // Ajout d'une limite pour le diagnostic comme dans le DTO
        public string Diagnostic { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)] // Ajout d'une limite pour les notes comme dans le DTO 
        public string Notes { get; set; } = string.Empty;

        // Liste des documents médicaux liés à cette consultation
        public virtual ICollection<DocumentMedical> Documents { get; set; } = new List<DocumentMedical>();
    }
}
