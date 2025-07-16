using ConsultationManagementService.Application.DTOs;
using MediatR;

namespace ConsultationManagementService.Application.Queries.GetConsultationsByClinicId
{
    public record GetConsultationsByClinicIdQuery(Guid ClinicId) : IRequest<List<ConsultationDTO>>;
}
