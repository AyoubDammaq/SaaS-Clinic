using MediatR;

namespace Doctor.Application.AvailibilityServices.Commands.SupprimerDisponibilitesParMedecinId
{
    public record SupprimerDisponibilitesParMedecinIdCommand(Guid medecinId) : IRequest;
}
