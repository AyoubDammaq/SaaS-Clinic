using PatientManagementService.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PatientManagementService.Domain.Entities
{
    public class Document : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;// e.g., PDF, Image, etc.

        [Required]
        public required string Url { get; set; } // File url

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public Guid DossierMedicalId { get; set; } // Foreign key to DossierMedical
        [JsonIgnore]
        public DossierMedical? DossierMedical { get; set; } // Navigation property

        public virtual void AttacherDocumentEvent(Document document)
        {
            AddDomainEvent(new Events.DocumentAttacheAuDossier(this, DossierMedicalId));
        }

        public virtual void DetacherDocumentEvent(Document document)
        {
            AddDomainEvent(new Events.DocumentDetacheAUnDossier(this, DossierMedicalId));

        }
    }
}
