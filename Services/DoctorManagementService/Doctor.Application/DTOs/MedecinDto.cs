using Doctor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Doctor.Application.DTOs
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
