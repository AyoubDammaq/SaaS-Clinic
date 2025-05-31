using Clinic.Application.DTOs;
using MediatR;

namespace Clinic.Application.Queries.GetStatistiquesDesCliniques
{
    public record GetStatistiquesDesCliniquesQuery(Guid cliniqueId) : IRequest<StatistiqueCliniqueDTO>;
}
