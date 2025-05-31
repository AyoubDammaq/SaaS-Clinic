using MediatR;
using RDV.Domain.Entities;

namespace RDV.Application.Queries.GetRendezVousByPatientId
{
    public record GetRendezVousByPatientIdQuery(Guid patientId) : IRequest<IEnumerable<RendezVous>>;
}
