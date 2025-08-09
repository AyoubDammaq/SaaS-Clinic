using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.TarificationService.Commands.Update
{
    public record UpdateCommand(UpdateTarificationRequest request) : IRequest;
}
