using Doctor.Application.DTOs;
using Doctor.Domain.Entities;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.AddDoctor
{
    public record AddDoctorCommand(CreateMedecinDto createMedecinDto) : IRequest;
}
