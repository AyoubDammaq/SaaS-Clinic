using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetNombreMedecinBySpecialiteDansUneClinique
{
    public record GetNombreMedecinBySpecialiteDansUneCliniqueQuery(Guid cliniqueId) : IRequest<IEnumerable<StatistiqueMedecinDTO>>;
}
