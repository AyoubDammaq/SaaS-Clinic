using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetTotalAvailableTime
{
    public record GetTotalAvailableTimeQuery(Guid medecinId, DateTime dateDebut, DateTime dateFin) : IRequest<TimeSpan>;
}
