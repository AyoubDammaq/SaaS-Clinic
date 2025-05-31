using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetAllFacturesByPatientId
{
    public record GetAllFacturesByPatientIdQuery(Guid patientId) : IRequest<IEnumerable<GetFacturesResponse>>;
}
