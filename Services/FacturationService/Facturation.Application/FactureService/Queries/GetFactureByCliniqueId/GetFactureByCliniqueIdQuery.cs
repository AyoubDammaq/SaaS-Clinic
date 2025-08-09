using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetFactureByCliniqueId
{
    public record GetFactureByCliniqueIdQuery(Guid cliniqueId) : IRequest<IEnumerable<GetFacturesResponse>>;
}
