using MediatR;

namespace RDV.Application.Queries.CountPendingRDVByDoctor
{
    public record CountPendingRDVByDoctorQuery(Guid medecinId) : IRequest<int>;
}
