using OfficeOpenXml;
using Reporting.Application.DTOs;

namespace Reporting.Application.Utils
{
    public static class DashboardExcelGenerator
    {
        public static byte[] Generate(DashboardStatsDTO stats)
        {
            // Définir le contexte de licence non commercial (EPPlus)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Dashboard");

            sheet.Cells["A1"].Value = "Statistique";
            sheet.Cells["B1"].Value = "Valeur";

            var data = new Dictionary<string, object>
            {
                ["Consultations"] = stats.ConsultationsJour,
                ["Nouveaux Patients"] = stats.NouveauxPatients,
                ["Nombre Factures"] = stats.NombreFactures,
                ["Total Factures Payées"] = stats.TotalFacturesPayees,
                ["Total Factures Impayées"] = stats.TotalFacturesImpayees,
                ["Paiements Payés"] = stats.PaiementsPayes,
                ["Paiements Impayés"] = stats.PaiementsImpayes,
                ["Paiements En Attente"] = stats.PaiementsEnAttente,
            };

            int row = 2;
            foreach (var item in data)
            {
                sheet.Cells[row, 1].Value = item.Key;
                sheet.Cells[row, 2].Value = item.Value;
                row++;
            }

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            return package.GetAsByteArray();
        }
    }
}
