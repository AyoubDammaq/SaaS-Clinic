using Facturation.Application.DTOs;
using MediatR;

namespace Facturation.Application.PaiementService.Commands.PayerFacture
{
    public record PayerFactureCommand(Guid factureId, PaiementDto PaiementDto) : IRequest<bool>;
}
