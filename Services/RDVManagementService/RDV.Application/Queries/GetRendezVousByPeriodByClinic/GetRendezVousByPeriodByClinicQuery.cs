using MediatR;
using RDV.Application.DTOs;

namespace RDV.Application.Queries.GetRendezVousByPeriodByClinic
{
    public record GetRendezVousByPeriodByClinicQuery(Guid cliniqueId, DateTime start, DateTime end) : IRequest<IEnumerable<RendezVousStatDTO>>;
}
