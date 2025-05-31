using Doctor.Domain.Entities;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.ObtenirDisponibilitesDansIntervalle
{
    public record ObtenirDisponibilitesDansIntervalleQuery(Guid medecinId, DateTime start, DateTime end) : IRequest<List<Disponibilite>>;
}
