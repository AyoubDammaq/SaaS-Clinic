using MediatR;

namespace Clinic.Application.Queries.GetNombreNouveauxCliniques
{
    public record GetNombreNouveauxCliniquesQuery(DateTime DateDebut, DateTime DateFin) : IRequest<int>;
}
