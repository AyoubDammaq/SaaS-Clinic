using System.ComponentModel.DataAnnotations;

namespace PatientManagementService.Application.DTOs
{
    public class CreatePatientDTO
    {
        [Required]
        [MinLength(2)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        public DateTime DateNaissance { get; set; }

        [Required]
        [RegularExpression("^(M|F)$", ErrorMessage = "Le sexe doit être 'M' ou 'F'.")]
        public string Sexe { get; set; } = string.Empty;

        [Required]
        [MinLength(5)]
        public string Adresse { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Telephone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
