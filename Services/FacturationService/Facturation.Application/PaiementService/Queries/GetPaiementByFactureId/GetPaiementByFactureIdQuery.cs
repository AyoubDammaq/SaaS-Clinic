using Facturation.Domain.Entities;
using MediatR;

namespace Facturation.Application.PaiementService.Queries.GetPaiementByFactureId
{
    public record GetPaiementByFactureIdQuery(Guid factureId) : IRequest<Paiement?>;
}
