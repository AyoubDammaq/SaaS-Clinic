using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetCreneauxDisponibles
{
    public record GetCreneauxDisponiblesQuery(Guid MedecinId, DateTime Date) : IRequest<List<CreneauDisponibleDto>>;
}
