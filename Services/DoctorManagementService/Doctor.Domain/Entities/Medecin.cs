using Doctor.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor.Domain.Entities
{
    public class Medecin : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Specialite { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; }

        public string Telephone { get; set; }

        public Guid? CliniqueId { get; set; }

        public string PhotoUrl { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        // Nouvelle propriété pour les disponibilités
        public ICollection<Disponibilite> Disponibilites { get; set; } = new List<Disponibilite>();

        // ✳️ Méthode pour déclencher un Domain Event
        public void AddDoctorEvent()
        {
            AddDomainEvent(new Events.DoctorCreated(this));
        }

        public void UpdateDoctorEvent()
        {
            AddDomainEvent(new Events.DoctorUpdated(this));
        }

        public void RemoveDoctorEvent()
        {
            AddDomainEvent(new Events.DoctorRemoved(this));
        }

        public virtual void AssignerCliniqueEvent(Guid cliniqueId)
        {
            AddDomainEvent(new Events.DoctorAssignedToClinique(this.Id, cliniqueId));
        }

        public void DesabonnerDeCliniqueEvent(Guid cliniqueId)
        {
            AddDomainEvent(new Events.DoctorUnassignedFromClinique(this.Id, cliniqueId));
        }

    }
}
