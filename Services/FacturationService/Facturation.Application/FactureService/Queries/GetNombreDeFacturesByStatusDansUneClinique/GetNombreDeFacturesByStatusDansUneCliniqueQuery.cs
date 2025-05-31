using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusDansUneClinique
{
    public record GetNombreDeFacturesByStatusDansUneCliniqueQuery(Guid cliniqueId) : IRequest<IEnumerable<FactureStatsDTO>>;
}
