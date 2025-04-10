using System.ComponentModel.DataAnnotations;

namespace DoctorManagementService.Models
{
    public class Medecin
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Specialite { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; }

        public string Telephone { get; set; }

        public Guid? CliniqueId { get; set; }

        public string PhotoUrl { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        // Nouvelle propriété pour les disponibilités
        public ICollection<Disponibilite> Disponibilites { get; set; } = new List<Disponibilite>();
    }
}
