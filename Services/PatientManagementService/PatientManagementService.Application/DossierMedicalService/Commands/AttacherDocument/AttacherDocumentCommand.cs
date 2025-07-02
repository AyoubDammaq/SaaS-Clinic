using MediatR;
using PatientManagementService.Application.DTOs;

namespace PatientManagementService.Application.DossierMedicalService.Commands.AttacherDocument
{
    public record AttacherDocumentCommand(Guid dossierMedicalId, CreateDocumentRequest document) : IRequest;
}
