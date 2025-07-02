using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RDV.Domain.Enums;
using RDV.Domain.Common;

namespace RDV.Domain.Entities
{
    public class RendezVous : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }  // Association avec le microservice Patient

        [Required]
        public Guid MedecinId { get; set; }  // Association avec le microservice Médecin

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateHeure { get; set; }

        [Required]
        [EnumDataType(typeof(RDVstatus))]
        public RDVstatus Statut { get; set; }  // Ex: "Confirmé", "Annulé", "En attente"

        [MaxLength(500)]
        public string Commentaire { get; set; } = string.Empty;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateCreation { get; set; } = DateTime.Now;

        public void CreerRendezVousEvent()
        {
            AddDomainEvent(new Events.RendezVousCree(this));
        }

        public void ModifierRendezVousEvent()
        {
            AddDomainEvent(new Events.RendezVousModifie(this));
        }

        public void AnnulerRendezVousEvent()
        {
            AddDomainEvent(new Events.RendezVousAnnule(this));
        }

        public void ConfirmerRendezVousEvent()
        {
            AddDomainEvent(new Events.RendezVousConfirmed(this));
        }

        public void RejeterRendezVousParMedecinEvent(string raison)
        {
            AddDomainEvent(new Events.RendezVousAnnuleParMedecin(this, raison));

        }
    }
}
