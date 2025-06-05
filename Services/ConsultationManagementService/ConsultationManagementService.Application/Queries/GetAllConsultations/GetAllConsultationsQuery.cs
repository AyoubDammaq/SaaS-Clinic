using MediatR;
using ConsultationManagementService.Domain.Entities;

namespace ConsultationManagementService.Application.Queries.GetAllConsultations
{
    public record GetAllConsultationsQuery() : IRequest<IEnumerable<Consultation>>;
}
