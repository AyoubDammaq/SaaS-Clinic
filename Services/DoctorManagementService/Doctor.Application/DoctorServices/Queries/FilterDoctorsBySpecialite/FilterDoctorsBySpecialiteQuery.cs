using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.FilterDoctorsBySpecialite
{
    public record FilterDoctorsBySpecialiteQuery(string specialite, int page = 1, int pageSize = 10) : IRequest<IEnumerable<GetMedecinRequestDto>>;
}
