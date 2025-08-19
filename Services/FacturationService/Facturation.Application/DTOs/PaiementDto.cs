using Facturation.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Facturation.Application.DTOs
{
    public class PaiementDto
    {
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ModePaiement MoyenPaiement { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à zéro.")]
        public decimal Montant { get; set; }

        public CardDetailsDto? CardDetails { get; set; }
    }
}
