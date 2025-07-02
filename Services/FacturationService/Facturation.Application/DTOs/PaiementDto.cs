using Facturation.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Facturation.Application.DTOs
{
    public class PaiementDto
    {
        [Required]
        public ModePaiement MoyenPaiement { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à zéro.")]
        public decimal Montant { get; set; }
    }
}
