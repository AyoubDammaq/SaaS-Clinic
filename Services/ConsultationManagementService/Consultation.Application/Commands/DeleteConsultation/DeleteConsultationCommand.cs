using MediatR;

namespace Consultation.Application.Commands.DeleteConsultation
{
    public record DeleteConsultationCommand(Guid id) : IRequest<bool>;
}
