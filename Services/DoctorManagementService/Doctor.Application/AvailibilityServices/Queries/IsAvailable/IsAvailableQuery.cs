using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.IsAvailable
{
    public record IsAvailableQuery(Guid medecinId, DateTime dateTime) : IRequest<bool>;
}
