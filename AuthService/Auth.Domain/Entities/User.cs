using System.ComponentModel.DataAnnotations;

namespace AuthentificationService.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PasswordHashed { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
