using MediatR;

namespace RDV.Application.Queries.CountPendingRDVByClinic
{
    public record CountPendingRDVByClinicQuery(Guid clinicId) : IRequest<int>;
}
