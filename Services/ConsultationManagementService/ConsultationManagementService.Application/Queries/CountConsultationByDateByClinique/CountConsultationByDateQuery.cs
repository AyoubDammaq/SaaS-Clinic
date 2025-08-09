using MediatR;

namespace ConsultationManagementService.Application.Queries.CountConsultationByDate
{
    public record CountConsultationByDateQuery(
        Guid? CliniqueId,
        Guid? MedecinId,
        Guid? PatientId,
        DateTime DateDebut,
        DateTime DateFin) : IRequest<int>;
}
