using Doctor.Domain.Common;
using Doctor.Domain.Events.DisponibilityEvents;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Doctor.Domain.Entities
{
    public class Disponibilite : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public DayOfWeek Jour { get; set; }

        [Required]
        public TimeSpan HeureDebut { get; set; }

        [Required]
        public TimeSpan HeureFin { get; set; }

        public Guid MedecinId { get; set; }
        [JsonIgnore]
        public Medecin? Medecin { get; set; }

        // ✅ Ajouter un événement après la création de la disponibilité
        public void AjouterDisponibiliteEvent()
        {
            AddDomainEvent(new DisponibiliteAjoutee(this.Id, this.MedecinId));
        }

        // ✅ Ajouter un événement après suppression de la disponibilité
        public virtual void SupprimerDisponibiliteEvent()
        {
            AddDomainEvent(new DisponibiliteSupprimee(this.Id));
        }

        // ✅ Ajouter un événement après mise à jour
        public void ModifierDisponibiliteEvent()
        {
            AddDomainEvent(new DisponibiliteModifiee(this.Id, this.MedecinId));
        }

        // ✅ Ajouter un événement global quand toutes les dispos d’un médecin sont supprimées
        public static Disponibilite SupprimerToutesPourMedecinEvent(Guid medecinId)
        {
            var dummy = new Disponibilite { MedecinId = medecinId };
            dummy.AddDomainEvent(new DisponibilitesSupprimeesParMedecin(medecinId));
            return dummy;
        }
    }
}
