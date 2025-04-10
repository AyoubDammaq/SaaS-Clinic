using DoctorManagementService.Models;
using System.ComponentModel.DataAnnotations;

namespace DoctorManagementService.DTOs
{
    public class MedecinDto
    {
        [Required]
        public string Prenom { get; set; }
        [Required]
        public string Nom { get; set; }
        [Required]
        public string Specialite { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public Guid? CliniqueId { get; set; }
        public string PhotoUrl { get; set; }
        public List<Disponibilite> Disponibilites { get; set; }
    }
}
