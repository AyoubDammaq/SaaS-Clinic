using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetNouveauxMedecins
{
    public record GetNouveauxMedecinsQuery(DateTime StartDate, DateTime EndDate) : IRequest<int>;
}
