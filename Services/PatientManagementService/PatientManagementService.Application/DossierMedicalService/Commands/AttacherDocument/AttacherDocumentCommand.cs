using MediatR;
using PatientManagementService.Application.DTOs;

namespace PatientManagementService.Application.DossierMedicalService.Commands.AttacherDocument
{
    public record AttacherDocumentCommand(Guid dossierMedicalId, DocumentDTO document) : IRequest;
}
