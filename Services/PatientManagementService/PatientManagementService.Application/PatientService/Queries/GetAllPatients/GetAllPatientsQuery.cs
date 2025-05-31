using MediatR;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.PatientService.Queries.GetAllPatients
{
    public record GetAllPatientsQuery() : IRequest<IEnumerable<Patient>>;
}
