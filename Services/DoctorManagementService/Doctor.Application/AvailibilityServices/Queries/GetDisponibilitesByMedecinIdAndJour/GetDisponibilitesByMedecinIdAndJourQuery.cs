using Doctor.Domain.Entities;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetDisponibilitesByMedecinIdAndJour
{
    public record GetDisponibilitesByMedecinIdAndJourQuery(Guid medecinId, DayOfWeek jour) : IRequest<List<Disponibilite>>;
}
