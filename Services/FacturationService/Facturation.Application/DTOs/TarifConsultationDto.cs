using Facturation.Domain.Entities;

namespace Facturation.Application.DTOs
{
    public class TarifConsultationDto
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public TypeConsultation ConsultationType { get; set; }

        public decimal Prix { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}
