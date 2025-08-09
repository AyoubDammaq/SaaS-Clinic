using MediatR;

namespace Facturation.Application.TarificationService.Commands.Delete
{
    public record DeleteCommand(Guid Id) : IRequest;
}
