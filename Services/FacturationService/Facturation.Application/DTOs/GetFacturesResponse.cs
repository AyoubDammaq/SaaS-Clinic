
using Facturation.Domain.Enums;

namespace Facturation.Application.DTOs
{
    public class GetFacturesResponse
    {
        public Guid PatientId { get; set; }

        public Guid ConsultationId { get; set; }

        public Guid ClinicId { get; set; }

        public DateTime DateEmission { get; set; } = DateTime.Now;

        public decimal MontantTotal { get; set; }

        public FactureStatus Status { get; set; }
    }
}
