using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PatientManagementService.Models
{
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenom { get; set; }

        [Required]
        public DateTime DateNaissance { get; set; }

        [Required]
        public string Sexe { get; set; } 

        public string Adresse { get; set; }

        public string NumeroTelephone { get; set; }

        public string Email { get; set; }

        public string DossierMedicalId { get; set; } 

        public DossierMedical DossierMedical { get; set; }
    }
}
