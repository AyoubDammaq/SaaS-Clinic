using Clinic.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs
{
    public class CliniqueDto
    {
        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Adresse { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string NumeroTelephone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Url]
        public string? SiteWeb { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public StatutClinique Statut { get; set; } = StatutClinique.Active;

        public TypeClinique TypeClinique { get; set; } = TypeClinique.Generale;
    }
}
