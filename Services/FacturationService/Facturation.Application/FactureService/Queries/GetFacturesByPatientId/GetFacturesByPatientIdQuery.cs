using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetFacturesByPatientId
{
    public record GetFacturesByPatientIdQuery(Guid patientId) : IRequest<IEnumerable<GetFacturesResponse>>;
}
