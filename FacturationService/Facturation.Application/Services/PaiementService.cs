using Facturation.Application.Interfaces;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using Aspose.Pdf;
using Aspose.Pdf.Text;


namespace Facturation.Application.Services
{
    public class PaiementService : IPaiementService
    {
        private readonly IPaiementRepository _paiementRepository;
        private readonly IFactureRepository _factureRepository;

        public PaiementService(IPaiementRepository paiementRepo, IFactureRepository factureRepo)
        {
            _paiementRepository = paiementRepo;
            _factureRepository = factureRepo;
        }

        public async Task<bool> PayerFactureAsync(Guid factureId, ModePaiement moyenPaiement)
        {
            var facture = await _factureRepository.GetFactureByIdAsync(factureId);

            if (facture == null || facture.Status == FactureStatus.PAYEE)
                return false;

            var paiement = new Paiement
            {
                FactureId = factureId,
                DatePaiement = DateTime.Now,
                Mode = moyenPaiement,
                Montant = facture.MontantTotal
            };

            facture.Status = FactureStatus.PAYEE;

            await _paiementRepository.AddAsync(paiement);
            await _factureRepository.UpdateFactureAsync(facture);

            return true;
        }

        public Task<byte[]> ImprimerRecuDePaiement(Paiement paiement)
        {
            using (var ms = new MemoryStream())
            {
                // Créer un nouveau document PDF
                var document = new Document();
                var page = document.Pages.Add();

                // Définir une police en gras
                var titleFont = FontRepository.FindFont("Helvetica");
                var titleTextState = new TextState
                {
                    Font = titleFont,
                    FontSize = 20,
                    FontStyle = FontStyles.Bold
                };

                // Ajouter un titre centré
                var title = new TextFragment("Reçu de Paiement");
                title.TextState.Font = FontRepository.FindFont("Helvetica");
                title.TextState.FontSize = 20;
                title.TextState.FontStyle = FontStyles.Bold;
                title.HorizontalAlignment = HorizontalAlignment.Center;
                page.Paragraphs.Add(title);

                // Ajouter les informations de paiement
                page.Paragraphs.Add(new TextFragment($"Date du paiement : {paiement.DatePaiement:dd/MM/yyyy}"));
                page.Paragraphs.Add(new TextFragment($"Montant : {paiement.Montant} TND"));
                page.Paragraphs.Add(new TextFragment($"Mode de paiement : {paiement.Mode}"));
                page.Paragraphs.Add(new TextFragment($"Facture ID : {paiement.FactureId}"));

                // Ajouter le message final
                page.Paragraphs.Add(new TextFragment("Merci pour votre paiement."));

                // Sauvegarder dans le MemoryStream
                document.Save(ms);
                return Task.FromResult(ms.ToArray());
            }
        }
        public async Task<Paiement?> GetPaiementByFactureIdAsync(Guid factureId)
        {
            return await _paiementRepository.GetByFactureIdAsync(factureId);
        }   
    }

}
