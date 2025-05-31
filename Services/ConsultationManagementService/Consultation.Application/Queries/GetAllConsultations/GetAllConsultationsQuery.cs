using MediatR;
using ConsultationManagementService.Models;

namespace Consultation.Application.Queries.GetAllConsultations
{
    public record GetAllConsultationsQuery() : IRequest<IEnumerable<ConsultationManagementService.Models.Consultation>>;
}
