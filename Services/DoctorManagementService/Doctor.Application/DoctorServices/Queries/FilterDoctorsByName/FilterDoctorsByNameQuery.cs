using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.FilterDoctorsByName
{
    public record FilterDoctorsByNameQuery(string name, string prenom, int page = 1, int pageSize = 10) : IRequest<IEnumerable<GetMedecinRequestDto>>;
}
