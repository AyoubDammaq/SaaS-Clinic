using System.ComponentModel.DataAnnotations;

namespace AuthentificationService.Models
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
