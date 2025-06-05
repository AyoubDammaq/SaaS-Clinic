using ConsultationManagementService.Domain.Entities;
using MediatR;

namespace ConsultationManagementService.Application.Queries.GetConsultationsByDoctorId
{
    public record GetConsultationsByDoctorIdQuery(Guid doctorId) : IRequest<IEnumerable<Consultation>>;
}
