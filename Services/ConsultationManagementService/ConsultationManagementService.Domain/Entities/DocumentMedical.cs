using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ConsultationManagementService.Domain.Common;
using ConsultationManagementService.Domain.Events;

namespace ConsultationManagementService.Domain.Entities
{
    public class DocumentMedical : BaseEntity
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

        public void UploadMedicalDocumentEvent()
        {
            AddDomainEvent(new MedicalDocumentUploaded(this));
        }

        public void RemoveMedicalDocumentEvent()
        {
            AddDomainEvent(new MedicalDocumentRemoved(this));
        }
    }
}