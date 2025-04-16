using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PatientManagementService.Models
{
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        public string NumeroTelephone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public Guid? DossierMedicalId { get; set; } 

        public DossierMedical? DossierMedical { get; set; }
    }
}
