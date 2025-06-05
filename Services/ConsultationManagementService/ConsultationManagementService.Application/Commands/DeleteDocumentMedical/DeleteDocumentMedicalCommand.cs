using MediatR;

namespace ConsultationManagementService.Application.Commands.DeleteDocumentMedical
{
    public record DeleteDocumentMedicalCommand(Guid id) : IRequest<bool>;
}
