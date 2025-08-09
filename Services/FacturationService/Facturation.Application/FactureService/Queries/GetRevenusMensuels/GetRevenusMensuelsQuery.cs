using MediatR;

namespace Facturation.Application.FactureService.Queries.GetRevenusMensuels
{
    public record GetRevenusMensuelsQuery(Guid clinicId) : IRequest<decimal>;
}
