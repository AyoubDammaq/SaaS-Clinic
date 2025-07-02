using Facturation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace Facturation.Application.PaiementService.Commands.ImprimerRecuDePaiement
{
    public class ImprimerRecuDePaiementCommandHandler : IRequestHandler<ImprimerRecuDePaiementCommand, byte[]>
    {
        private readonly IPaiementRepository _paiementRepository;
        private readonly ILogger<ImprimerRecuDePaiementCommandHandler> _logger;
        public ImprimerRecuDePaiementCommandHandler(IPaiementRepository paiementRepository, ILogger<ImprimerRecuDePaiementCommandHandler> logger)
        {
            _paiementRepository = paiementRepository ?? throw new ArgumentNullException(nameof(paiementRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public Task<byte[]> Handle(ImprimerRecuDePaiementCommand request, CancellationToken cancellationToken)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                if (request.paiement == null)
                    throw new ArgumentNullException(nameof(request.paiement), "Le paiement à imprimer est null.");

                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Size(PageSizes.A5);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .Text("REÇU DE PAIEMENT")
                            .FontSize(20)
                            .Bold()
                            .AlignCenter();

                        page.Content().Column(column =>
                        {
                            column.Spacing(15);

                            column.Item().Text(txt =>
                            {
                                txt.Span("Date du paiement : ").SemiBold();
                                txt.Span($"{request.paiement.DatePaiement:dd/MM/yyyy}");
                            });

                            column.Item().Text(txt =>
                            {
                                txt.Span("Montant payé : ").SemiBold();
                                txt.Span($"{request.paiement.Montant:N2} TND").FontColor(Colors.Green.Medium);
                            });

                            column.Item().Text(txt =>
                            {
                                txt.Span("Mode de paiement : ").SemiBold();
                                txt.Span($"{request.paiement.Mode}");
                            });

                            column.Item().Text(txt =>
                            {
                                txt.Span("Facture ID : ").SemiBold();
                                txt.Span($"{request.paiement.FactureId}");
                            });

                            column.Item().PaddingTop(20).AlignCenter().Text("Merci pour votre paiement.")
                                .Italic().FontSize(11).FontColor(Colors.Grey.Darken2);
                        });

                        page.Footer().AlignCenter().Text("Généré automatiquement - QuestPDF").FontSize(9).Italic().FontColor(Colors.Grey.Medium);
                    });
                });

                using var stream = new MemoryStream();
                document.GeneratePdf(stream);
                return Task.FromResult(stream.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'impression du reçu de paiement pour la facture {FactureId}", request.paiement?.FactureId);
                throw new ApplicationException("Erreur lors de l'impression du reçu de paiement.");
            }
        }
    }
}