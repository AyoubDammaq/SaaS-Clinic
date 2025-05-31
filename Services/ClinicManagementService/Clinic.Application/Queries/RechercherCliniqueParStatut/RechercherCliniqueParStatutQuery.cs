using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using MediatR;

namespace Clinic.Application.Queries.RechercherCliniqueParStatut
{
    public record RechercherCliniqueParStatusQuery(StatutClinique statut) : IRequest<IEnumerable<Clinique>>;
}
