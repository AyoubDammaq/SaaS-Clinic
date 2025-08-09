using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.TarificationService.Commands.Add
{
    public record AddCommand(AddTarificationRequest request) : IRequest;
}
