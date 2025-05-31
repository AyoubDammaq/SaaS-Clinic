using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetNombreDeFactureParClinique
{
    public record GetNombreDeFactureParCliniqueQuery() : IRequest<IEnumerable<FactureStatsDTO>>;
}
