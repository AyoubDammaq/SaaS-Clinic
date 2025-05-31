using MediatR;

namespace Consultation.Application.Queries.GetConsultationsByDoctorId
{
    public record GetConsultationsByDoctorIdQuery(Guid doctorId) : IRequest<IEnumerable<ConsultationManagementService.Models.Consultation>>;
}
