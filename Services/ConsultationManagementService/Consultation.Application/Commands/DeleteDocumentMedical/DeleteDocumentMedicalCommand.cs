using MediatR;

namespace Consultation.Application.Commands.DeleteDocumentMedical
{
    public record DeleteDocumentMedicalCommand(Guid id) : IRequest<bool>;
}
