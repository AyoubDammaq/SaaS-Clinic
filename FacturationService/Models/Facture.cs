using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FacturationService.Models
{
    public class Facture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; } 

        [Required]
        public DateTime DateEmission { get; set; } = DateTime.Now;

        [Required]
        public decimal MontantTotal { get; set; }

        [Required]
        public FactureStatus EstPayee { get; set; }
    }
}
