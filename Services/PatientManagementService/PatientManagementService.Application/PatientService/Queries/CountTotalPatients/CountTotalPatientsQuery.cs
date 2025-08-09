using MediatR;

namespace PatientManagementService.Application.PatientService.Queries.CountTotalPatients
{
    public record CountTotalPatientsQuery() : IRequest<int>;
}
