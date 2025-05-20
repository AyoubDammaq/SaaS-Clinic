using Doctor.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Doctor.Application.DTOs
{
    public class GetMedecinRequestDto
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
}
