using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Facturation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;

namespace Facturation.Application.FactureService.Commands.ExportToPdf
{
    public class ExportToPdfCommandHandler : IRequestHandler<ExportToPdfCommand, byte[]>
    {
        private readonly IFactureRepository _factureRepository;
        private readonly ILogger<ExportToPdfCommandHandler> _logger;
        public ExportToPdfCommandHandler(IFactureRepository factureRepository, ILogger<ExportToPdfCommandHandler> logger)
        {
            _factureRepository = factureRepository ?? throw new ArgumentNullException(nameof(factureRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public Task<byte[]> Handle(ExportToPdfCommand request, CancellationToken cancellationToken)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                if (request.facture == null)
                    throw new ArgumentNullException(nameof(request.facture), "La facture à exporter est null.");

                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Size(PageSizes.A4);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .Text("FACTURE")
                            .FontSize(20)
                            .Bold()
                            .AlignCenter();

                        page.Content()
                            .Column(col =>
                            {
                                col.Spacing(5);

                                col.Item().Text($"Facture ID: {request.facture.Id}");
                                col.Item().Text($"Patient ID: {request.facture.PatientId}");
                                col.Item().Text($"Consultation ID: {request.facture.ConsultationId}");
                                col.Item().Text($"Clinic ID: {request.facture.ClinicId}");
                                col.Item().Text($"Date d'émission: {request.facture.DateEmission:dd/MM/yyyy}");
                                col.Item().Text($"Montant total: {request.facture.MontantTotal:N2} TND");
                                col.Item().Text($"Statut: {request.facture.Status}");
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text("Document généré automatiquement - QuestPDF")
                            .FontSize(10)
                            .Italic();
                    });
                });

                using var stream = new MemoryStream();
                document.GeneratePdf(stream);
                return Task.FromResult(stream.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'export PDF de la facture {FactureId}", request.facture?.Id);
                throw new ApplicationException("Erreur lors de la génération du PDF de la facture.", ex);
            }
        }
    }
}
