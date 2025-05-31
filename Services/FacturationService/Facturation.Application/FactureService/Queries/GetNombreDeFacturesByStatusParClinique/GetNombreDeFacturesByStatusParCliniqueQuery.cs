using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusParClinique
{
    public record GetNombreDeFacturesByStatusParCliniqueQuery() : IRequest<IEnumerable<FactureStatsDTO>>;
}
