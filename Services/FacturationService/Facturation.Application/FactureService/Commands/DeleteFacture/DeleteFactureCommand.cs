using MediatR;

namespace Facturation.Application.FactureService.Commands.DeleteFacture
{
    public record DeleteFactureCommand(Guid id) : IRequest;
}
