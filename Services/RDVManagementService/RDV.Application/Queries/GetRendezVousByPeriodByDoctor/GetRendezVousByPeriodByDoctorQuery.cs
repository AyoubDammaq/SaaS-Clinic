using MediatR;
using RDV.Application.DTOs;

namespace RDV.Application.Queries.GetRendezVousByPeriodByDoctor
{
    public record GetRendezVousByPeriodByDoctorQuery(Guid MedecinId, DateTime Start, DateTime End) : IRequest<IEnumerable<RendezVousStatDTO>>;
}
