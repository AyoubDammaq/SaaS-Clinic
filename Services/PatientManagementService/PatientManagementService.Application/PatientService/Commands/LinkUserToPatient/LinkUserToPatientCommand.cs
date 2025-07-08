using MediatR;
using PatientManagementService.Application.DTOs;

namespace PatientManagementService.Application.PatientService.Commands.LinkUserToPatient
{
    public record LinkUserToPatientCommand(LinkDto LinkDto) : IRequest<bool>;
}
