using System.ComponentModel.DataAnnotations;

namespace AuthentificationService.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PasswordHashed { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        // Références vers entités externes (selon le rôle)
        public Guid? CliniqueId { get; set; } // pour ClinicAdmin
        public Guid? MedecinId { get; set; }  // pour Doctor
        public Guid? PatientId { get; set; }  // pour Patient

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiryTime { get; set; }
    }
}
