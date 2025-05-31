using Aspose.Pdf.Text;
using Facturation.Domain.Interfaces;
using MediatR;
using Aspose.Pdf;

namespace Facturation.Application.FactureService.Commands.ExportToPdf
{
    public class ExportToPdfCommandHandler : IRequestHandler<ExportToPdfCommand, byte[]>
    {
        private readonly IFactureRepository _factureRepository;
        public ExportToPdfCommandHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository ?? throw new ArgumentNullException(nameof(factureRepository));
        }
        public async Task<byte[]> Handle(ExportToPdfCommand request, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                var document = new Document();
                var page = document.Pages.Add();

                var title = new TextFragment("FACTURE")
                {
                    TextState = { FontSize = 20, FontStyle = FontStyles.Bold }
                };
                page.Paragraphs.Add(title);

                page.Paragraphs.Add(new TextFragment($"Facture ID: {request.facture.Id}"));
                page.Paragraphs.Add(new TextFragment($"Patient ID: {request.facture.PatientId}"));
                page.Paragraphs.Add(new TextFragment($"Consultation ID: {request.facture.ConsultationId}"));
                page.Paragraphs.Add(new TextFragment($"Clinic ID: {request.facture.ClinicId}"));
                page.Paragraphs.Add(new TextFragment($"Date d'émission: {request.facture.DateEmission:dd/MM/yyyy}"));
                page.Paragraphs.Add(new TextFragment($"Montant total: {request.facture.MontantTotal:C}"));
                page.Paragraphs.Add(new TextFragment($"Statut: {request.facture.Status}"));

                using var stream = new MemoryStream();
                document.Save(stream);
                return stream.ToArray();
            });
        }
    }
}
