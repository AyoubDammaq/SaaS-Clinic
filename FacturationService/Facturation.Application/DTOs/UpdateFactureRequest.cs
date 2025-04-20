
namespace Facturation.Application.DTOs
{
    public class UpdateFactureRequest
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid ConsultationId { get; set; }
        public Guid ClinicId { get; set; }
        public decimal MontantTotal { get; set; }
    }
}
