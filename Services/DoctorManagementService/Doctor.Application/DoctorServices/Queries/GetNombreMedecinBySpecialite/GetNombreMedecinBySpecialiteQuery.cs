using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetNombreMedecinBySpecialite
{
    public record GetNombreMedecinBySpecialiteQuery : IRequest<IEnumerable<StatistiqueMedecinDTO>>;
}
