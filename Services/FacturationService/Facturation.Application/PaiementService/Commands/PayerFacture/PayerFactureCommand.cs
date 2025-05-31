using Facturation.Domain.Enums;
using MediatR;

namespace Facturation.Application.PaiementService.Commands.PayerFacture
{
    public record PayerFactureCommand(Guid factureId, ModePaiement moyenPaiement) : IRequest<bool>;
}
