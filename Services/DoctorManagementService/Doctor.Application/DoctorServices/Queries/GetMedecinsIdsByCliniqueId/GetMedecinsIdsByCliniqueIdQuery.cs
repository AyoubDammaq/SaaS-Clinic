using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetMedecinsIdsByCliniqueId
{
    public record GetMedecinsIdsByCliniqueIdQuery(Guid cliniqueId) : IRequest<IEnumerable<Guid>>;
}
