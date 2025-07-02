using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetStatistiquesFacturesParPeriode
{
    public record GetStatistiquesFacturesParPeriodeQuery(DateTime DateDebut, DateTime DateFin)
     : IRequest<StatistiquesFacturesDto>;
}
