using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetMedecinByClinique
{
    public record GetMedecinByCliniqueQuery(Guid cliniqueId) : IRequest<IEnumerable<GetMedecinRequestDto>>;
}
