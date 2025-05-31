using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.UpdateDoctor
{
    public record UpdateDoctorCommand(Guid id, MedecinDto medecinDto) : IRequest;
}
