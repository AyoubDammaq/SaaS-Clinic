using PatientManagementService.Application.DTOs;
using MediatR;

namespace PatientManagementService.Application.PatientService.Commands.UpdatePatient
{
    public record UpdatePatientCommand(PatientDTO patient) : IRequest<bool>;
}
