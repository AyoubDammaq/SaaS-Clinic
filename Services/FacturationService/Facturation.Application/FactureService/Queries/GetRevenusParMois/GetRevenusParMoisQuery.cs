using MediatR;

namespace Facturation.Application.FactureService.Queries.GetRevenusParMois
{
    public record GetRevenusParMoisQuery(Guid ClinicId) : IRequest<Dictionary<int, decimal>>;
}
