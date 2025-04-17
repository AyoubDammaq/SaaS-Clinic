using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Doctor.Domain.Entities
{
    public class Disponibilite
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public DayOfWeek Jour { get; set; }

        [Required]
        public TimeSpan HeureDebut { get; set; }

        [Required]
        public TimeSpan HeureFin { get; set; }

        [JsonIgnore]
        public Guid MedecinId { get; set; }
        [JsonIgnore]
        public Medecin? Medecin { get; set; }
    }
}
