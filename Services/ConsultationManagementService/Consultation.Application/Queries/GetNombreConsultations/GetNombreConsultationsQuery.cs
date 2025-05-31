using MediatR;

namespace Consultation.Application.Queries.GetNombreConsultations
{
    public record GetNombreConsultationsQuery(DateTime dateDebut, DateTime dateFin) : IRequest<int>;
}
