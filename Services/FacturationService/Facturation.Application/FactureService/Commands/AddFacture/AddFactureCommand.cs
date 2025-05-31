using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.FactureService.Commands.AddFacture
{
    public record AddFactureCommand(CreateFactureRequest request) : IRequest;
}
