using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetNombreDeFactureByStatus
{
    public record GetNombreDeFactureByStatusQuery() : IRequest<IEnumerable<FactureStatsDTO>>;
}
