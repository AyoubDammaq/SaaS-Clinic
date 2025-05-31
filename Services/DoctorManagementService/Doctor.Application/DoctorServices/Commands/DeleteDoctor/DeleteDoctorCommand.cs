using MediatR;

namespace Doctor.Application.DoctorServices.Commands.DeleteDoctor
{
    public record DeleteDoctorCommand(Guid id) : IRequest;
}
