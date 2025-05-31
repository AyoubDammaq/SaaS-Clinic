using Doctor.Domain.Entities;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetDisponibilitesByMedecinId
{
    public record GetDisponibilitesByMedecinIdQuery(Guid medecinId) : IRequest<List<Disponibilite>>;
}
