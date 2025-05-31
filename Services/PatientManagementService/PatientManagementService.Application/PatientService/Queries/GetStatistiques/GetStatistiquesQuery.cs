using MediatR;

namespace PatientManagementService.Application.PatientService.Queries.GetStatistiques
{
    public record GetStatistiquesQuery(DateTime dateDebut, DateTime dateFin) : IRequest<int>;
}
