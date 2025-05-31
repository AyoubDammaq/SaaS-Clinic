using MediatR;

namespace Clinic.Application.Queries.GetNombreNouvellesCliniquesDuMois
{
    public record GetNombreNouvellesCliniquesDuMoisQuery() : IRequest<int>;
}
