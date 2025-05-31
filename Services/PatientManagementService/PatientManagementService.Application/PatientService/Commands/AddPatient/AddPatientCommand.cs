using MediatR;
using PatientManagementService.Application.DTOs;

namespace PatientManagementService.Application.PatientService.Commands.AddPatient
{
    public record AddPatientCommand(PatientDTO patient) : IRequest<bool>;
}
