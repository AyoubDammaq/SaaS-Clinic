using Facturation.Domain.Enums;

namespace Facturation.Application.DTOs
{
    public class FactureUpdatedDTO
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid ConsultationId { get; set; }
        public Guid ClinicId { get; set; }
        public DateTime DateEmission { get; set; }
        public decimal MontantTotal { get; set; }
        public decimal MontantPaye { get; set; }
        public FactureStatus Status { get; set; }
    }
}
