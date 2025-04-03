using System.ComponentModel.DataAnnotations;

namespace DoctorManagementService.Models
{
    public class Medecin
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Specialite { get; set; } = string.Empty;

        public Guid CliniqueId { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        private Medecin() { }
        public Medecin(Guid id, string prenom, string nom, string specialite, Guid cliniqueId)
        {
            Id = id;
            Prenom = prenom;
            Nom = nom;
            Specialite = specialite;
            CliniqueId = cliniqueId;
        }

        public Guid GetId() => Id;
        public void SetId(Guid id) => Id = id;

        public string GetPrenom() => Prenom;
        public void SetPrenom(string prenom) => Prenom = prenom;

        public string GetNom() => Nom;
        public void SetNom(string nom) => Nom = nom;

        public string GetSpecialite() => Specialite;
        public void SetSpecialite(string specialite) => Specialite = specialite;

        public Guid GetCliniqueId() => CliniqueId;
        public void SetCliniqueId(Guid cliniqueId) => CliniqueId = cliniqueId;

        public DateTime GetDateCreation() => DateCreation;
        public void SetDateCreation(DateTime dateCreation) => DateCreation = dateCreation;
    }
}
