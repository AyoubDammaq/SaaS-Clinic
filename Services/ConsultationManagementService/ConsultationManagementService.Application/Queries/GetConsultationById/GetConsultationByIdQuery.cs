using ConsultationManagementService.Domain.Entities;
using MediatR;

namespace ConsultationManagementService.Application.Queries.GetConsultationById
{
    public record GetConsultationByIdQuery(Guid id) : IRequest<Consultation?>;
}
