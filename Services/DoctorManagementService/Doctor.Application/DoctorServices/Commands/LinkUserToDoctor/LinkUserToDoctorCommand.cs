using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.LinkUserToDoctor
{
    public record LinkUserToDoctorCommand(LinkDTO LinkDTO) : IRequest<bool>;
}
