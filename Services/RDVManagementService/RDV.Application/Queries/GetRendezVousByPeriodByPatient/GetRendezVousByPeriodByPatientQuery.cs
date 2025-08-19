using MediatR;
using RDV.Application.DTOs;

namespace RDV.Application.Queries.GetRendezVousByPeriodByPatient
{
    public record GetRendezVousByPeriodByPatientQuery(Guid PatientId, DateTime Start, DateTime End) : IRequest<IEnumerable<RendezVousStatDTO>>;
}
