using Facturation.Application.DTOs;
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
        private readonly IUnitOfWork _unitOfWork;

        public PayerFactureCommandHandler(
            IPaiementRepository paiementRepository,
            IFactureRepository factureRepository,
            ILogger<PayerFactureCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _paiementRepository = paiementRepository;
            _factureRepository = factureRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<bool> Handle(PayerFactureCommand request, CancellationToken cancellationToken)
        {
            var facture = await _factureRepository.GetFactureByIdAsync(request.factureId);
            if (facture == null || facture.Status == FactureStatus.PAYEE)
                return false;

            var montantRestant = facture.MontantTotal - facture.MontantPaye;
            var paiementDto = request.PaiementDto;
            if (paiementDto.Montant <= 0 || paiementDto.Montant > montantRestant)
                throw new InvalidOperationException("Le montant payé est invalide (trop élevé ou nul).");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var paiement = new Paiement
                {
                    FactureId = request.factureId,
                    DatePaiement = DateTime.Now,
                    Mode = paiementDto.MoyenPaiement,
                    Montant = paiementDto.Montant
                };

                // Gérer les détails de la carte pour CarteBancaire
                if (paiementDto.MoyenPaiement == ModePaiement.CarteBancaire)
                {
                    if (paiementDto.CardDetails == null)
                        throw new InvalidOperationException("Détails de la carte requis pour un paiement par carte.");

                    if (!IsValidCardDetails(paiementDto.CardDetails))
                        throw new InvalidOperationException("Détails de la carte invalides.");

                    paiement.CardDetails = new CardDetails
                    {
                        CardholderName = paiementDto.CardDetails.CardholderName,
                        CardNumber = paiementDto.CardDetails.CardNumber.Replace(" ", ""),
                        ExpiryDate = paiementDto.CardDetails.ExpiryDate,
                        Cvv = paiementDto.CardDetails.Cvv,
                        PaiementId = paiement.Id
                    };
                }

                facture.MontantPaye += paiementDto.Montant;
                if (facture.MontantPaye == facture.MontantTotal)
                    facture.Status = FactureStatus.PAYEE;
                else
                    facture.Status = FactureStatus.PARTIELLEMENT_PAYEE;

                paiement.PayerFactureEvent();
                facture.UpdateFactureEvent();

                await _paiementRepository.AddAsync(paiement);
                await _factureRepository.UpdateFactureAsync(facture);

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Erreur lors du paiement de la facture {FactureId}", request.factureId);
                throw; // Renvoyer pour remonter le 500
            }
        }

        private bool IsValidCardDetails(CardDetailsDto cardDetails)
        {
            if (string.IsNullOrEmpty(cardDetails.CardNumber) || cardDetails.CardNumber.Length < 16 || cardDetails.CardNumber.Length > 19)
                return false;

            // Vérification du format de la date d'expiration (MM/AA)
            if (!System.Text.RegularExpressions.Regex.IsMatch(cardDetails.ExpiryDate, @"^(0[1-9]|1[0-2])\/([0-9]{2})$"))
                return false;

            // Vérification de l'expiration future
            var expiryParts = cardDetails.ExpiryDate.Split('/');
            var expiryMonth = int.Parse(expiryParts[0]);
            var expiryYear = int.Parse("20" + expiryParts[1]); // Suppose AA comme 20XX
            var expiryDate = new DateTime(expiryYear, expiryMonth, 1).AddMonths(1).AddDays(-1); // Dernier jour du mois
            if (expiryDate < DateTime.Now)
                return false;

            // Vérification de la longueur du CVV
            if (cardDetails.Cvv.Length < 3 || cardDetails.Cvv.Length > 4)
                return false;

            // Algorithme de Luhn (simplifié pour vérifier la validité du numéro de carte)
            string cleanNumber = new string(cardDetails.CardNumber.Where(char.IsDigit).ToArray());
            int sum = 0;
            bool alternate = false;
            for (int i = cleanNumber.Length - 1; i >= 0; i--)
            {
                int digit = int.Parse(cleanNumber[i].ToString());
                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9) digit -= 9;
                }
                sum += digit;
                alternate = !alternate;
            }
            if (sum % 10 != 0)
                return false;

            return true;
        }
    }
}