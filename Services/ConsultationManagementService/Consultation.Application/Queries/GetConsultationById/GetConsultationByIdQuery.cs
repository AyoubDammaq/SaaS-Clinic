using MediatR;

namespace Consultation.Application.Queries.GetConsultationById
{
    public record GetConsultationByIdQuery(Guid id) : IRequest<ConsultationManagementService.Models.Consultation?>;
}
