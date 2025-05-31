using MediatR;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.DossierMedicalService.Queries.GetDossierMedicalById
{
    public record GetDossierMedicalByIdQuery(Guid Id) : IRequest<DossierMedical>;
}
