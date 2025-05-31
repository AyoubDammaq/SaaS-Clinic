using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.FilterDoctorsByName
{
    public record FilterDoctorsByNameQuery(string name, string prenom) : IRequest<IEnumerable<GetMedecinRequestDto>>;
}
