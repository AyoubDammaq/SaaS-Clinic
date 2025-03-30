using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RessourceManagementService.Models
{
    public class RessourceMedicale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public ressourceType Type { get; set; } // Ex: "Équipement", "Salle", "Médicament"

        [Required]
        public int QuantiteDisponible { get; set; }

        public bool EstDisponible { get; set; } = true;
    }
}
