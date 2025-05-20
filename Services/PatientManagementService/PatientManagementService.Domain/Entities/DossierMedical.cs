using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PatientManagementService.Domain.Entities
{
    public class DossierMedical
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
        public List<Document>? Documents { get; set; } = new List<Document>();
        public DateTime DateCreation { get; set; }
        public Guid PatientId { get; set; } // Foreign key to Patient
        public Patient? Patient { get; set; } // Navigation property to Patient

    }
}