using MediatR;

namespace ConsultationManagementService.Application.Queries.GetNombreConsultations
{
    public record GetNombreConsultationsQuery(DateTime dateDebut, DateTime dateFin) : IRequest<int>;
}
