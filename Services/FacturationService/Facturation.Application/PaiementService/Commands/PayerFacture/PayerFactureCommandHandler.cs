using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.PaiementService.Commands.PayerFacture
{
    public class PayerFactureCommandHandler : IRequestHandler<PayerFactureCommand, bool>
    {
        private readonly IFactureRepository _factureRepository;
        private readonly IPaiementRepository _paiementRepository;
        private readonly ILogger<PayerFactureCommandHandler> _logger;
        public PayerFactureCommandHandler(IPaiementRepository paiementRepository, IFactureRepository factureRepository, ILogger<PayerFactureCommandHandler> logger)
        {
            _paiementRepository = paiementRepository;
            _factureRepository = factureRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<bool> Handle(PayerFactureCommand request, CancellationToken cancellationToken)
        {
            var facture = await _factureRepository.GetFactureByIdAsync(request.factureId);

            if (facture == null || facture.Status == FactureStatus.PAYEE)
                return false;

            var montantRestant = facture.MontantTotal - facture.MontantPaye;

            if (request.montant <= 0 || request.montant > montantRestant)
                throw new InvalidOperationException("Le montant payé est invalide (trop élevé ou nul).");

            try
            {
                var paiement = new Paiement
                {
                    FactureId = request.factureId,
                    DatePaiement = DateTime.Now,
                    Mode = request.moyenPaiement,
                    Montant = request.montant
                };

                facture.MontantPaye += request.montant;

                if (facture.MontantPaye == facture.MontantTotal)
                    facture.Status = FactureStatus.PAYEE;
                else
                    facture.Status = FactureStatus.PARTIELLEMENT_PAYEE;

                paiement.PayerFactureEvent();
                facture.UpdateFactureEvent();

                await _paiementRepository.AddAsync(paiement);
                await _factureRepository.UpdateFactureAsync(facture);

                return true;
            }
            catch (Exception ex)
            {
                // ✅ Ajoute un log explicite ici
                _logger.LogError(ex, "Erreur lors du paiement de la facture {FactureId}", facture.Id);
                throw; // Renvoyer pour remonter le 500
            }
        }
    }
}
