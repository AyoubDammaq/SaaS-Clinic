using Doctor.Domain.Entities;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Commands.AjouterDisponibilite
{
    public record AjouterDisponibiliteCommand(Disponibilite nouvelleDispo) : IRequest;
}
