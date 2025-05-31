using Doctor.Domain.Entities;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetDisponibilites
{
    public record GetDisponibilitesQuery() : IRequest<List<Disponibilite>>;
}
