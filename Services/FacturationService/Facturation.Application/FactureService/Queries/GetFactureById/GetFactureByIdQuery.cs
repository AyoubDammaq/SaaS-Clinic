using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetFactureById
{
    public record GetFactureByIdQuery(Guid id) : IRequest<FactureDto>;
}
