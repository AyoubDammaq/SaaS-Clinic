using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetRevenusMensuelTrend
{
    public record GetRevenusMensuelTrendQuery(Guid clinicId) : IRequest<RevenusMensuelTrendDto>;
}
