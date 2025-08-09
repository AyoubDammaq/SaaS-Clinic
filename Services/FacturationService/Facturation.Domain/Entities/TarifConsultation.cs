namespace Facturation.Domain.Entities
{
    public class TarifConsultation
    {
        public Guid Id { get; set; }

        public Guid ClinicId { get; set; } 
        public TypeConsultation ConsultationType { get; set; } 

        public decimal Prix { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
    public enum TypeConsultation
    {
        ConsultationGenerale = 1,
        ConsultationSpecialiste = 2,
        ConsultationUrgence = 3,
        ConsultationSuivi = 4,
        ConsultationLaboratoire = 5
    }
}
