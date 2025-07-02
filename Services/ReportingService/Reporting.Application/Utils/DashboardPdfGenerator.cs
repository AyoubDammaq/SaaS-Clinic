using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Reporting.Application.DTOs;

namespace Reporting.Application.Utils
{
    public static class DashboardPdfGenerator
    {
        public static byte[] Generate(DashboardStatsDTO stats)
        {
            if (stats == null)
                throw new ArgumentNullException(nameof(stats));

            return QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text("Statistiques du Dashboard").Bold().FontSize(20).AlignCenter();

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Item().Text($"Consultations : {stats.ConsultationsJour}");
                        col.Item().Text($"Nouveaux Patients : {stats.NouveauxPatients}");
                        col.Item().Text($"Nombre de Factures : {stats.NombreFactures}");

                        col.Item().Text($"Factures Payées : {stats.TotalFacturesPayees:N2} dt");
                        col.Item().Text($"Factures Impayées : {stats.TotalFacturesImpayees:N2} dt");

                        col.Item().Text($"Paiements Payés : {stats.PaiementsPayes:N2} dt");
                        col.Item().Text($"Paiements Impayés : {stats.PaiementsImpayes:N2} dt");
                        col.Item().Text($"Paiements en Attente : {stats.PaiementsEnAttente:N2} dt");
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Généré par ReportingService - ");
                        x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                    });
                });
            }).GeneratePdf();
        }
    }
}
