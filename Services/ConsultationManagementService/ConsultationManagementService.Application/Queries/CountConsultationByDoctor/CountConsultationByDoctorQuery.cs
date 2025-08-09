using MediatR;

namespace ConsultationManagementService.Application.Queries.CountConsultationByDoctor
{
    public record CountConsultationByDoctorQuery(Guid MedecinId, DateTime? startDate, DateTime? endDate) : IRequest<int>;
}
