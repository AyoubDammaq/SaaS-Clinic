using MediatR;

namespace ConsultationManagementService.Application.Queries.CountByMedecinIds
{
    public record CountByMedecinIdsQuery(List<Guid> medecinIds) : IRequest<int>;
}
