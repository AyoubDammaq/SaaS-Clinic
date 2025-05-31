using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Queries.RechercherCliniqueParAdresse
{
    public record RechercherCliniqueParAdresseQuery(string Adresse) : IRequest<IEnumerable<Clinique>>;
}
