using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Queries.ListerClinique
{
    public record ListerCliniquesQuery() : IRequest<List<Clinique>>;
}
