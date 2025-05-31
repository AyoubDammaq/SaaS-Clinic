using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.FilterDoctorsBySpecialite
{
    public record FilterDoctorsBySpecialiteQuery(string specialite) : IRequest<IEnumerable<GetMedecinRequestDto>>;
}
