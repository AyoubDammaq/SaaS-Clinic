using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DoctorManagementService.Models
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

        public Guid MedecinId { get; set; }
        public Medecin? Medecin { get; set; }
    }
}