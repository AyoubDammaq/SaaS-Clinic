using MediatR;

namespace Clinic.Application.Queries.GetNombreCliniques
{
    public record GetNombreCliniquesQuery() : IRequest<int>;
}
