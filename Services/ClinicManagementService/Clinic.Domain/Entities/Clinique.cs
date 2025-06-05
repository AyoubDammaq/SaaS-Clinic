using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.Domain.Entities
{
    public class Clinique : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Adresse { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string NumeroTelephone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Url]
        public string? SiteWeb { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public StatutClinique Statut { get; set; } = StatutClinique.Active;

        public TypeClinique TypeClinique { get; set; } = TypeClinique.Generale;

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        // ✳️ Méthode pour déclencher un Domain Event
        public void AjouterCliniqueEvent()
        {
            AddDomainEvent(new Events.CliniqueCreated(this));
        }

        public void ModifierCliniqueEvent()
        {
            AddDomainEvent(new Events.CliniqueUpdated(this));
        }

        public void SupprimerCliniqueEvent()
        {
            AddDomainEvent(new Events.CliniqueDeleted(this));
        }
    }
}
