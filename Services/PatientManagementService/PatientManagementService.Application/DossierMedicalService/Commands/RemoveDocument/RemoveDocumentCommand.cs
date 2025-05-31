
using MediatR;

namespace PatientManagementService.Application.DossierMedicalService.Commands.RemoveDocument
{
    public record RemoveDocumentCommand(Guid documentId) : IRequest;
}
