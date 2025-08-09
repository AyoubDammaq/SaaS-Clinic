using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetFactureByConsultationId
{
    public record GetFactureByConsultationIdQuery(Guid consultationId)
    : IRequest<FactureDto>;
}
