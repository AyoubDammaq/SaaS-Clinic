using MediatR;

namespace ConsultationManagementService.Application.Queries.CountNouveauxPatientsByDoctor
{
    public record CountNouveauxPatientsByDoctorQuery(Guid medecinId, DateTime startDate, DateTime endDate) : IRequest<int>;
}
