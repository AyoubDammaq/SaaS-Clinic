using Doctor.Domain.Entities;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.AddDoctor
{
    public record AddDoctorCommand(Medecin medecin) : IRequest;
}
