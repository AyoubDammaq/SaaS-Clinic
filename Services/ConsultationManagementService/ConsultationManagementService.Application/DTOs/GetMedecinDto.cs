using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConsultationManagementService.Application.DTOs
{
    public class GetMedecinDto
    {
        public Guid Id { get; set; }
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

    public class Disponibilite
    {
    }
}
