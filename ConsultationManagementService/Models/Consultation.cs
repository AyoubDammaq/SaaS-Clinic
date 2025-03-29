using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConsultationManagementService.Models
{
    public class Consultation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }  // Lien avec le microservice Gestion des Patients

        [Required]
        public Guid MedecinId { get; set; }  // Lien avec le microservice Gestion des Médecins

        [Required]
        public DateTime DateConsultation { get; set; }

        public string Diagnostic { get; set; }

        public string Notes { get; set; }

        // Liste des documents médicaux liés à cette consultation
        public List<DocumentMedical> Documents { get; set; } = new List<DocumentMedical>();
    }
}
