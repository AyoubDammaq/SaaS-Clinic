using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetAllFacturesByRangeOfDate
{
    public record GetAllFacturesByRangeOfDateQuery(DateTime startDate, DateTime endDate) : IRequest<IEnumerable<GetFacturesResponse>>;
}
