using Facturation.Domain.Entities;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.PaiementService.Queries.GetPaiementByFactureId
{
    public class GetPaiementByFactureIdQueryHandler : IRequestHandler<GetPaiementByFactureIdQuery, Paiement?>
    {
        private readonly IPaiementRepository _paiementRepository;
        public GetPaiementByFactureIdQueryHandler(IPaiementRepository paiementRepository)
        {
            _paiementRepository = paiementRepository;
        }
        public async Task<Paiement?> Handle(GetPaiementByFactureIdQuery request, CancellationToken cancellationToken)
        {
            return await _paiementRepository.GetByFactureIdAsync(request.factureId);
        }
    }
}
