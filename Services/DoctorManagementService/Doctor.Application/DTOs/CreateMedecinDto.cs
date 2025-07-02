using Doctor.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Doctor.Application.DTOs
{
    public class CreateMedecinDto
    {
        [Required]
        public string Prenom { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Specialite { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Telephone { get; set; }

        public string PhotoUrl { get; set; }

        public List<Disponibilite> Disponibilites { get; set; } = new();
    }

}
