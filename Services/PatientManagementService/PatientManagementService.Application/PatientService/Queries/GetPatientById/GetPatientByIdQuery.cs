using MediatR;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.PatientService.Queries.GetPatientById
{
    public record GetPatientByIdQuery(Guid id) : IRequest<Patient?>;
}
