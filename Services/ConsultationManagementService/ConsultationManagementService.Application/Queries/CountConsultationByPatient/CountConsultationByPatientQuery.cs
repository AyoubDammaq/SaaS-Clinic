using MediatR;

namespace ConsultationManagementService.Application.Queries.CountConsultationByPatient
{
    public record CountConsultationByPatientQuery(Guid PatientId, DateTime? startDate, DateTime? endDate) : IRequest<int>;
}
