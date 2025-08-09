using MediatR;

namespace ConsultationManagementService.Application.Queries.CountNouveauxPatientsByClinic
{
    public record CountNouveauxPatientsByClinicQuery(Guid ClinicId, DateTime startDate, DateTime endDate) : IRequest<int>;
}
