using MediatR;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.DossierMedicalService.Queries.GetAllDossiersMedicals
{
    public record GetAllDossiersMedicalsQuery() : IRequest<IEnumerable<DossierMedical>>;

}
