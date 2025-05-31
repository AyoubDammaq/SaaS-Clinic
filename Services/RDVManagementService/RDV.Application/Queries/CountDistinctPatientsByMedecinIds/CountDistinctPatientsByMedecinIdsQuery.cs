using MediatR;

namespace RDV.Application.Queries.CountDistinctPatientsByMedecinIds
{
    public record CountDistinctPatientsByMedecinIdsQuery(List<Guid> medecinIds) : IRequest<int>;
}
