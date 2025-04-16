using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PatientManagementService.Models
{
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;// e.g., PDF, Image, etc.

        [Required]
        public required byte[] Contenu { get; set; } // File content as binary data

        [JsonIgnore]
        public Guid DossierMedicalId { get; set; } // Foreign key to DossierMedical
        [JsonIgnore]
        public DossierMedical? DossierMedical { get; set; } // Navigation property
    }
}
