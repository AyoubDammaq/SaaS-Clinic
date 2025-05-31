using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Commands.UpdateFacture
{
    public record UpdateFactureCommand(UpdateFactureRequest request) : IRequest;
}
