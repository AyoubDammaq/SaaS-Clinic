using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetNombreMedecinByClinique
{
    public record GetNombreMedecinByCliniqueQuery : IRequest<IEnumerable<StatistiqueMedecinDTO>>;
}
