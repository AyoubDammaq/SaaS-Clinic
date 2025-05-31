using Facturation.Application.DTOs;
using Facturation.Domain.Enums;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetAllFacturesByState
{
    public record GetAllFacturesByStateQuery(FactureStatus status) : IRequest<IEnumerable<GetFacturesResponse>>;
}
