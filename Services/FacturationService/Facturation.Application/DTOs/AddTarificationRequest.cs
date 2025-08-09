using Facturation.Domain.Entities;

namespace Facturation.Application.DTOs
{
    public class AddTarificationRequest
    {
        public Guid ClinicId { get; set; }
        public TypeConsultation ConsultationType { get; set; }

        public decimal Prix { get; set; }
    }
}
