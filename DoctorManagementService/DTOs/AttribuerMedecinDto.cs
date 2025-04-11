using System.ComponentModel.DataAnnotations;

namespace DoctorManagementService.DTOs
{
    public class AttribuerMedecinDto
    {
        [Required]
        public Guid MedecinId { get; set; }

        [Required]
        public Guid CliniqueId { get; set; }
    }
}