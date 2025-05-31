using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using MediatR;


namespace Clinic.Application.Queries.RechercherCliniqueParType
{
    public record RechercherCliniqueParTypeQuery(TypeClinique type) : IRequest<IEnumerable<Clinique>>;
}
