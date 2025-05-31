using MediatR;

namespace Doctor.Application.DoctorServices.Commands.AttribuerMedecinAUneClinique
{
    public record AttribuerMedecinAUneCliniqueCommand(Guid medecinId, Guid cliniqueId) : IRequest;
}
