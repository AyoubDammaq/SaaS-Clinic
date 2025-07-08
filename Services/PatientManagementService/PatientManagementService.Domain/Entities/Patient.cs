using System.ComponentModel.DataAnnotations;
using PatientManagementService.Domain.Common;
using PatientManagementService.Domain.Events;

namespace PatientManagementService.Domain.Entities
{
    public class Patient : BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        public DateTime DateNaissance { get; set; }

        [Required]
        public string Sexe { get; set; } = string.Empty;

        public string Adresse { get; set; } = string.Empty;

        public string Telephone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        public Guid? DossierMedicalId { get; set; }

        public virtual DossierMedical? DossierMedical { get; set; }

        public Guid? UserId { get; set; }

        // Domain event methods
        public virtual void AjouterPatientEvent() =>
            AddDomainEvent(new PatientAdded(Id, Nom, Prenom));

        public virtual void ModifierPatientEvent() =>
            AddDomainEvent(new PatientUpdated(this));

        public virtual void SupprimerPatientEvent() =>
            AddDomainEvent(new PatientDeleted(Id));
    }
}
