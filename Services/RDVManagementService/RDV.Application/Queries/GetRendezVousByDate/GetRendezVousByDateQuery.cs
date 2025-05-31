using MediatR;
using RDV.Domain.Entities;

namespace RDV.Application.Queries.GetRendezVousByDate
{
    public record GetRendezVousByDateQuery(DateTime date) : IRequest<IEnumerable<RendezVous>>;
}
