using MediatR;

namespace Doctor.Application.AvailibilityServices.Commands.SupprimerDisponibilite
{
    public record SupprimerDisponibiliteCommand(Guid disponibiliteId) : IRequest;
}
