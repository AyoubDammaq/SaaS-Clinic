using MediatR;

namespace Consultation.Application.Queries.CountByMedecinIds
{
    public record CountByMedecinIdsQuery(List<Guid> medecinIds) : IRequest<int>;
}
