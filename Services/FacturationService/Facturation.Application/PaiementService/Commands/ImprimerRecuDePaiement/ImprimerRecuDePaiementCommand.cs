using Facturation.Domain.Entities;
using MediatR;

namespace Facturation.Application.PaiementService.Commands.ImprimerRecuDePaiement
{
    public record ImprimerRecuDePaiementCommand(Paiement paiement) : IRequest<byte[]>;
}
