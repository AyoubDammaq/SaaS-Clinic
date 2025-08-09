using MediatR;

namespace ConsultationManagementService.Application.Queries.CountConsultationByClinic
{
    public record CountConsultationByClinicQuery(Guid ClinicId, DateTime? startDate, DateTime? endDate) : IRequest<int>;
}
