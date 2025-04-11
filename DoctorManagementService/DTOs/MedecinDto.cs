using DoctorManagementService.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public List<Disponibilite>? Disponibilites { get; set; }
    }
}
