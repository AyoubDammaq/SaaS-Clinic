using MediatR;

namespace RDV.Application.Queries.GetNombreRendezVousParCliniqueEtDate
{
    public record GetNombreRendezVousParCliniqueEtDateQuery(Guid cliniqueId, DateTime date) : IRequest<int>;
}
