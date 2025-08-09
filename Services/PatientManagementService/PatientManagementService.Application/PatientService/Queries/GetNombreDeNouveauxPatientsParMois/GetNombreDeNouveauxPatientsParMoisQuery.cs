using MediatR;

namespace PatientManagementService.Application.PatientService.Queries.GetNombreDeNouveauxPatientsParMois
{
    public record GetNombreDeNouveauxPatientsParMoisQuery(DateTime Date) : IRequest<Dictionary<string, int>>;
}
