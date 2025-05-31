using MediatR;

namespace PatientManagementService.Application.PatientService.Commands.DeletePatient
{
    public record DeletePatientCommand(Guid PatientId) : IRequest<bool>;   
}
