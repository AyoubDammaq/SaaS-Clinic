using Facturation.Domain.Entities;

namespace Facturation.Application.DTOs
{
    public class UpdateTarificationRequest
    {
        public Guid Id { get; set; }
        public TypeConsultation ConsultationType { get; set; }

        public decimal Prix { get; set; }
    }
}
