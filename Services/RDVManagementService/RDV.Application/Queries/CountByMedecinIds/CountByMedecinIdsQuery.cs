using MediatR;

namespace RDV.Application.Queries.CountByMedecinIds
{
    public record CountByMedecinIdsQuery(List<Guid> medecinIds) : IRequest<int>;
}
