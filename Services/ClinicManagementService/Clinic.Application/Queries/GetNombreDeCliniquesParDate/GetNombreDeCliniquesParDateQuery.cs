using MediatR;

namespace Clinic.Application.Queries.GetNombreDeCliniquesParDate
{
    public record GetNombreDeCliniquesParDateQuery(DateTime startDate, DateTime endDate) : IRequest<int>;
}
