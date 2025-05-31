using Aspose.Pdf.Text;
using Facturation.Domain.Interfaces;
using MediatR;
using Aspose.Pdf;

namespace Facturation.Application.PaiementService.Commands.ImprimerRecuDePaiement
{
    public class ImprimerRecuDePaiementCommandHandler : IRequestHandler<ImprimerRecuDePaiementCommand, byte[]>
    {
        private readonly IPaiementRepository _paiementRepository;
        public ImprimerRecuDePaiementCommandHandler(IPaiementRepository paiementRepository)
        {
            _paiementRepository = paiementRepository ?? throw new ArgumentNullException(nameof(paiementRepository));
        }
        public async Task<byte[]> Handle(ImprimerRecuDePaiementCommand request, CancellationToken cancellationToken)
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
                page.Paragraphs.Add(new TextFragment($"Date du paiement : {request.paiement.DatePaiement:dd/MM/yyyy}"));
                page.Paragraphs.Add(new TextFragment($"Montant : {request.paiement.Montant} TND"));
                page.Paragraphs.Add(new TextFragment($"Mode de paiement : {request.paiement.Mode}"));
                page.Paragraphs.Add(new TextFragment($"Facture ID : {request.paiement.FactureId}"));

                // Ajouter le message final
                page.Paragraphs.Add(new TextFragment("Merci pour votre paiement."));

                // Sauvegarder dans le MemoryStream
                document.Save(ms);
                return await Task.FromResult(ms.ToArray());
            }
        }
    }
}