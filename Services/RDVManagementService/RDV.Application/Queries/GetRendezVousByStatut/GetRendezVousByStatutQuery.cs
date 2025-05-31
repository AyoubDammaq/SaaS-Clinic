using MediatR;
using RDV.Domain.Entities;
using RDV.Domain.Enums;

namespace RDV.Application.Queries.GetRendezVousByStatut
{
    public record GetRendezVousByStatutQuery(RDVstatus statut) : IRequest<IEnumerable<RendezVous>>;
}
