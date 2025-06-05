using MediatR;

namespace ConsultationManagementService.Application.Commands.DeleteConsultation
{
    public record DeleteConsultationCommand(Guid id) : IRequest<bool>;
}
