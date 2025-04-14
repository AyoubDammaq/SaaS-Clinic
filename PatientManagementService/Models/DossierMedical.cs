using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PatientManagementService.Models
{
    public class DossierMedical
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Allergies { get; set; }
        public string MaladiesChroniques { get; set; }
        public string MedicamentsActuels { get; set; }
        public string AntécédentsFamiliaux { get; set; }
        public string AntécédentsPersonnels { get; set; }
        public string GroupeSanguin { get; set; }
        public DateTime DateCreation { get; set; }
        public Guid PatientId { get; set; } // Foreign key to Patient
        public Patient Patient { get; set; } // Navigation property to Patient

    }
}