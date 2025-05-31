using MediatR;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.DossierMedicalService.Queries.GetDossierMedicalByPatientId
{
    public record GetDossierMedicalByPatientIdQuery(Guid patientId) : IRequest<DossierMedical>; 
}
