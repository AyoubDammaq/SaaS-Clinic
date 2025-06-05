using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.PaiementService.Commands.PayerFacture
{
    public class PayerFactureCommandHandler : IRequestHandler<PayerFactureCommand, bool>
    {
        private readonly IFactureRepository _factureRepository;
        private readonly IPaiementRepository _paiementRepository;
        public PayerFactureCommandHandler(IPaiementRepository paiementRepository, IFactureRepository factureRepository)
        {
            _paiementRepository = paiementRepository;
            _factureRepository = factureRepository;
        }
        public async Task<bool> Handle(PayerFactureCommand request, CancellationToken cancellationToken)
        {
            var facture = await _factureRepository.GetFactureByIdAsync(request.factureId);

            if (facture == null || facture.Status == FactureStatus.PAYEE)
                return false;

            var paiement = new Paiement
            {
                FactureId = request.factureId,
                DatePaiement = DateTime.Now,
                Mode = request.moyenPaiement,
                Montant = facture.MontantTotal
            };

            facture.Status = FactureStatus.PAYEE;

            paiement.PayerFactureEvent();
            facture.UpdateFactureEvent();

            await _paiementRepository.AddAsync(paiement);
            await _factureRepository.UpdateFactureAsync(facture);

            return true;
        }
    }
}
