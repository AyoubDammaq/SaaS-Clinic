using System.ComponentModel.DataAnnotations;


namespace Doctor.Application.DTOs
{
    public class AttribuerMedecinDto
    {
        [Required]
        public Guid MedecinId { get; set; }

        [Required]
        public Guid CliniqueId { get; set; }
    }
}
