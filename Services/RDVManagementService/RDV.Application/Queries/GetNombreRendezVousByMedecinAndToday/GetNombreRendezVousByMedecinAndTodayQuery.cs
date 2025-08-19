using MediatR;

namespace RDV.Application.Queries.GetNombreRendezVousByMedecinAndToday
{
    public record GetNombreRendezVousByMedecinAndTodayQuery(Guid MedecinId, DateTime Date) : IRequest<int>;
}
