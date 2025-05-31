using MediatR;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.PatientService.Queries.GetPatientsByName
{
    public record GetPatientsByNameQuery(string? name, string? lastname) : IRequest<IEnumerable<Patient>>;
}
