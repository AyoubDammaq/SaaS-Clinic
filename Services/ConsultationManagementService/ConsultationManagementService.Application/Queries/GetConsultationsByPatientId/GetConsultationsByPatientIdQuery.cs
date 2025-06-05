using ConsultationManagementService.Domain.Entities;
using MediatR;

namespace ConsultationManagementService.Application.Queries.GetConsultationsByPatientId
{
    public record GetConsultationsByPatientIdQuery(Guid patientId) : IRequest<IEnumerable<Consultation>>;
}
