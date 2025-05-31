using Doctor.Domain.Entities;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Commands.UpdateDisponibilite
{
    public record UpdateDisponibiliteCommand(Guid disponibiliteId, Disponibilite disponibilite) : IRequest;
}
