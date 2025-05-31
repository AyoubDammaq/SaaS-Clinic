using MediatR;
using PatientManagementService.Application.DTOs;

namespace PatientManagementService.Application.DossierMedicalService.Commands.AddDossierMedical
{
    public record AddDossierMedicalCommand(DossierMedicalDTO dossierMedical) : IRequest;

}
