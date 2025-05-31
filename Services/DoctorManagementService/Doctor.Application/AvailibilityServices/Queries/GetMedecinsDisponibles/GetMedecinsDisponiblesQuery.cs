using Doctor.Domain.Entities;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetMedecinsDisponibles
{
    public record GetMedecinsDisponiblesQuery(DateTime date, TimeSpan? heureDebut, TimeSpan? heureFin) : IRequest<List<Medecin>>;
}
