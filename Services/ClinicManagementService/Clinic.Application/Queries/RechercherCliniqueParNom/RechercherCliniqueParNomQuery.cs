using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Queries.RechercherCliniqueParNom
{
    public record RechercherCliniqueParNomQuery(string Nom) : IRequest<IEnumerable<Clinique>>;

}
