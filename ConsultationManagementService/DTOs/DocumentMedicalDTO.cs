using ConsultationManagementService.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConsultationManagementService.DTOs
{
    public class DocumentMedicalDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ConsultationId { get; set; } 

        [Required]
        public string Type { get; set; }  

        [Required]
        public string FichierURL { get; set; } 

        public DateTime DateAjout { get; set; } = DateTime.Now;

    }
}
