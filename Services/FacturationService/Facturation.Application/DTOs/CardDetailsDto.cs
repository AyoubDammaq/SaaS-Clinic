using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Facturation.Application.DTOs
{
    public class CardDetailsDto
    {
        [Required]
        [StringLength(100)]
        public string CardholderName { get; set; } = string.Empty;

        [Required]
        [StringLength(19)]
        [JsonPropertyName("cardNumber")]
        public string CardNumber { get; set; } = string.Empty; // Sans espaces

        [Required]
        [StringLength(5)]
        public string ExpiryDate { get; set; } = string.Empty; // Format MM/AA

        [Required]
        [StringLength(4)]
        public string Cvv { get; set; } = string.Empty;
    }
}
