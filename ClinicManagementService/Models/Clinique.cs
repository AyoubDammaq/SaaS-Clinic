using System.ComponentModel.DataAnnotations;

namespace ClinicManagementService.Models
{
    public class Clinique
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nom { get; set; } = string.Empty;

        public string Adresse { get; set; } = string.Empty;

        public string Ville { get; set; } = string.Empty;

        public string Pays { get; set; } = string.Empty;

        public int CodePostal { get; set; } = 0;

        public string Téléphone { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}
