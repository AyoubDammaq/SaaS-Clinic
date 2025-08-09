using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetNombreTotalMedecins
{
    public record GetNombreTotalMedecinsQuery() : IRequest<int>;
}
