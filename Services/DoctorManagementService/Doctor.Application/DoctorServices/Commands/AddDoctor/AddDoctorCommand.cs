using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.AddDoctor
{
    public record AddDoctorCommand(CreateMedecinDto createMedecinDto) : IRequest<GetMedecinRequestDto>;
}
