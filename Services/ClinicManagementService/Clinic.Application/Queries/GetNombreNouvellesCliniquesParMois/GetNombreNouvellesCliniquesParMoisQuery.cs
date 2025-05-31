using Clinic.Application.DTOs;
using MediatR;

namespace Clinic.Application.Queries.GetNombreNouvellesCliniquesParMois
{
    public record GetNombreNouvellesCliniquesParMoisQuery : IRequest<IEnumerable<StatistiqueDTO>>;
}
