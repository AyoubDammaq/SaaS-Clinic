using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetAllDoctors
{
    public record GetAllDoctorsQuery() : IRequest<IEnumerable<GetMedecinRequestDto>>;
}
