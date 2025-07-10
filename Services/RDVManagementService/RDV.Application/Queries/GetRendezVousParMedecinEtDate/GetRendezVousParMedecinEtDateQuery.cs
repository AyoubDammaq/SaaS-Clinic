using MediatR;
using RDV.Domain.Entities;

namespace RDV.Application.Queries.GetRendezVousParMedecinEtDate
{
    public record GetRendezVousParMedecinEtDateQuery(Guid MedecinId, DateTime Date) : IRequest<IEnumerable<RendezVous>>;
}
