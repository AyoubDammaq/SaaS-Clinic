using MediatR;

namespace PatientManagementService.Application.DossierMedicalService.Commands.DeleteDossierMedical
{
    public record DeleteDossierMedicalCommand(Guid dossierMedicalId) : IRequest;

}
