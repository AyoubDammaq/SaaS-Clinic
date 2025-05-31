using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetAllFactures
{
    public record GetAllFacturesQuery() : IRequest<IEnumerable<GetFacturesResponse>>;
}
