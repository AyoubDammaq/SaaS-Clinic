using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PatientManagementService.Domain.Common;
using System.Text.Json.Serialization;

namespace PatientManagementService.Domain.Entities
{
    public class DossierMedical : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Allergies { get; set; } = string.Empty;
        public string MaladiesChroniques { get; set; } = string.Empty;
        public string MedicamentsActuels { get; set; } = string.Empty;
        public string AntécédentsFamiliaux { get; set; } = string.Empty;
        public string AntécédentsPersonnels { get; set; } = string.Empty;
        public string GroupeSanguin { get; set; } = string.Empty;
        public virtual List<Document>? Documents { get; set; } = new List<Document>();
        public DateTime DateCreation { get; set; }
        public Guid PatientId { get; set; } // Foreign key to Patient
        [JsonIgnore]
        public virtual Patient? Patient { get; set; } // Navigation property to Patient

        public void CreerDossierMedicalEvent()
        {
            AddDomainEvent(new Events.DossierMedicalCree(this));
        }

        public void ModifierDossierMedicalEvent()
        {
            AddDomainEvent(new Events.DossierMedicalModifie(this));
        }

        public void SupprimerDossierMedicalEvent()
        {
            AddDomainEvent(new Events.DossierMedicalSupprime(this.Id));
        }
    }
}