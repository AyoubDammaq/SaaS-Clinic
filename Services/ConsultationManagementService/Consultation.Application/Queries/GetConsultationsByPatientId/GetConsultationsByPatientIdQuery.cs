using MediatR;

namespace Consultation.Application.Queries.GetConsultationsByPatientId
{
    public record GetConsultationsByPatientIdQuery(Guid patientId) : IRequest<IEnumerable<ConsultationManagementService.Models.Consultation>>;
}
