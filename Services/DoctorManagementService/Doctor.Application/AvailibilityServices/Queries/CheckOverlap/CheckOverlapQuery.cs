using Doctor.Domain.Entities;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.CheckOverlap
{
    public record CheckOverlapQuery(Disponibilite dispo) : IRequest<bool>;
}
