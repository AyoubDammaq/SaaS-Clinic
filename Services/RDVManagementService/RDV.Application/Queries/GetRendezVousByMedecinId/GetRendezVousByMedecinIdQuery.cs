using MediatR;
using RDV.Domain.Entities;

namespace RDV.Application.Queries.GetRendezVousByMedecinId
{
    public record GetRendezVousByMedecinIdQuery(Guid medecinId) : IRequest<IEnumerable<RendezVous>>;
}
