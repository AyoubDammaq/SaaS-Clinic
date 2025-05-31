using MediatR;

namespace Doctor.Application.DoctorServices.Commands.DesabonnerMedecinDeClinique
{
    public record DesabonnerMedecinDeCliniqueCommand(Guid medecinId) : IRequest;
}
