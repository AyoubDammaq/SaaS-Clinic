using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetActivitesMedecin
{
    public record GetActivitesMedecinQuery(Guid medecinId) : IRequest<IEnumerable<ActiviteMedecinDTO>>;
}
