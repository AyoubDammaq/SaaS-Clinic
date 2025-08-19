using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetBillingStats
{
    public record GetBillingStatsQuery(Guid cliniqueId) : IRequest<BillingStatsDto>;
}
