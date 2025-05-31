using Doctor.Application.DTOs;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetDoctorById
{
    public record GetDoctorByIdQuery(Guid id) : IRequest<GetMedecinRequestDto>;
}
