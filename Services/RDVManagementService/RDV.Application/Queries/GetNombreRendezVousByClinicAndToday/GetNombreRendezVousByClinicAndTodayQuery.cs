using MediatR;

namespace RDV.Application.Queries.GetNombreRendezVousByClinicAndToday
{
    public record GetNombreRendezVousByClinicAndTodayQuery(Guid ClinicId, DateTime Date) : IRequest<int>;
}
