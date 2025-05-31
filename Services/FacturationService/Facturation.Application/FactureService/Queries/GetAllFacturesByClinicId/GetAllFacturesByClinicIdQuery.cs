using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetAllFacturesByClinicId
{
    public record GetAllFacturesByClinicIdQuery(Guid id) : IRequest<GetFacturesResponse>;
}
