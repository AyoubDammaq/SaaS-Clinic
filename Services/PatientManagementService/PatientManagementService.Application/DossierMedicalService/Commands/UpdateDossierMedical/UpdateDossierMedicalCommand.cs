using MediatR;
using PatientManagementService.Application.DTOs;

namespace PatientManagementService.Application.DossierMedicalService.Commands.UpdateDossierMedical
{
    public record UpdateDossierMedicalCommand(DossierMedicalDTO dossierMedical) : IRequest;
}
