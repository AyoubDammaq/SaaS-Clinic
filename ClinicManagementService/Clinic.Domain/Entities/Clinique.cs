using System.ComponentModel.DataAnnotations;

namespace Clinic.Domain.Entities
{
    public class Clinique
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Adresse { get; set; } = string.Empty;

        [MaxLength(20)]
        public string NumeroTelephone { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}
