using Doctor.Domain.Entities;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetDisponibiliteById
{
    public record GetDisponibiliteByIdQuery(Guid disponibiliteId) : IRequest<Disponibilite>;
}
