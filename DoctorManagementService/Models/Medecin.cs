using System.ComponentModel.DataAnnotations;

namespace DoctorManagementService.Models
{
    public class Medecin
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Specialite { get; set; } = string.Empty;

        public Guid CliniqueId { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}
