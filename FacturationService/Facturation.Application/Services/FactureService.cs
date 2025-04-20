using Facturation.Domain.Entities;
using Facturation.Domain.Interfaces;
using Facturation.Domain.Enums;
using Facturation.Application.DTOs;
using Aspose.Pdf.Text;
using Aspose.Pdf;

namespace Facturation.Application.Services
{
    public class FactureService : IFactureService
    {
        public readonly IFactureRepository _factureRepository;

        public FactureService(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository ?? throw new ArgumentNullException(nameof(factureRepository));
        }

        public async Task<GetFacturesResponse> GetFactureByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la facture ne peut pas être vide.", nameof(id));

            var facture = await _factureRepository.GetFactureByIdAsync(id);
            if (facture == null)
                throw new KeyNotFoundException($"Aucune facture trouvée avec l'identifiant {id}.");

            return new GetFacturesResponse
            {
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                DateEmission = facture.DateEmission,
                MontantTotal = facture.MontantTotal,
                Status = facture.Status
            };
        }

        public async Task<IEnumerable<GetFacturesResponse>> GetAllFacturesAsync()
        {
            var factures = await _factureRepository.GetAllFacturesAsync();
            if (!factures.Any())
                throw new InvalidOperationException("Aucune facture disponible.");

            return factures.Select(facture => new GetFacturesResponse
            {
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                DateEmission = facture.DateEmission,
                MontantTotal = facture.MontantTotal,
                Status = facture.Status
            });
        }

        public async Task AddFactureAsync(CreateFactureRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.MontantTotal <= 0)
                throw new ArgumentException("Le montant total de la facture doit être supérieur à zéro.", nameof(request.MontantTotal));

            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                ConsultationId = request.ConsultationId,
                ClinicId = request.ClinicId,
                DateEmission = DateTime.UtcNow,
                MontantTotal = request.MontantTotal,
                Status = FactureStatus.EN_ATTENTE
            };

            await _factureRepository.AddFactureAsync(facture);
        }

        public async Task UpdateFactureAsync(UpdateFactureRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la facture ne peut pas être vide.", nameof(request.Id));

            if (request.PatientId == Guid.Empty)
                throw new ArgumentException("L'identifiant du patient ne peut pas être vide.", nameof(request.PatientId));

            if (request.ConsultationId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.", nameof(request.ConsultationId));

            if (request.ClinicId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique ne peut pas être vide.", nameof(request.ClinicId));

            var existingFacture = await _factureRepository.GetFactureByIdAsync(request.Id);
            if (existingFacture == null)
                throw new KeyNotFoundException($"Aucune facture trouvée avec l'identifiant {request.PatientId}.");

            // Map UpdateFactureRequest to Facture entity directly
            existingFacture.PatientId = request.PatientId;
            existingFacture.ConsultationId = request.ConsultationId;
            existingFacture.ClinicId = request.ClinicId;
            existingFacture.MontantTotal = request.MontantTotal;

            await _factureRepository.UpdateFactureAsync(existingFacture);
        }


        public async Task DeleteFactureAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la facture ne peut pas être vide.", nameof(id));

            var existingFacture = await _factureRepository.GetFactureByIdAsync(id);
            if (existingFacture == null)
                throw new KeyNotFoundException($"Aucune facture trouvée avec l'identifiant {id}.");

            await _factureRepository.DeleteFactureAsync(id);
        }

        public async Task<IEnumerable<GetFacturesResponse>> GetAllFacturesByRangeOfDateAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("La date de début ne peut pas être postérieure à la date de fin.");

            var factures = await _factureRepository.GetAllFacturesByRangeOfDateAsync(startDate, endDate);
            if (!factures.Any())
                throw new InvalidOperationException("Aucune facture trouvée dans la plage de dates spécifiée.");

            return factures.Select(facture => new GetFacturesResponse
            {
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                DateEmission = facture.DateEmission,
                MontantTotal = facture.MontantTotal,
                Status = facture.Status
            });
        }

        public async Task<IEnumerable<GetFacturesResponse>> GetAllFacturesByStateAsync(FactureStatus status)
        {
            var factures = await _factureRepository.GetAllFacturesByStateAsync(status);
            if (!factures.Any())
                throw new InvalidOperationException($"Aucune facture trouvée avec le statut {status}.");

            return factures.Select(facture => new GetFacturesResponse
            {
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                DateEmission = facture.DateEmission,
                MontantTotal = facture.MontantTotal,
                Status = facture.Status
            });
        }

        public async Task<IEnumerable<GetFacturesResponse>> GetAllFacturesByPatientIdAsync(Guid patientId)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("L'identifiant du patient ne peut pas être vide.", nameof(patientId));

            var factures = await _factureRepository.GetAllFacturesByPatientIdAsync(patientId);
            if (!factures.Any())
                throw new InvalidOperationException($"Aucune facture trouvée pour le patient avec l'identifiant {patientId}.");

            return factures.Select(facture => new GetFacturesResponse
            {
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                DateEmission = facture.DateEmission,
                MontantTotal = facture.MontantTotal,
                Status = facture.Status
            });
        }

        public async Task<IEnumerable<GetFacturesResponse>> GetAllFacturesByClinicIdAsync(Guid clinicId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique ne peut pas être vide.", nameof(clinicId));

            var factures = await _factureRepository.GetAllFacturesByClinicIdAsync(clinicId);
            if (!factures.Any())
                throw new InvalidOperationException($"Aucune facture trouvée pour la clinique avec l'identifiant {clinicId}.");

            return factures.Select(facture => new GetFacturesResponse
            {
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                DateEmission = facture.DateEmission,
                MontantTotal = facture.MontantTotal,
                Status = facture.Status
            });
        }

        public async Task<byte[]> ExportToPdfAsync(Facture facture)
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

                page.Paragraphs.Add(new TextFragment($"Facture ID: {facture.Id}"));
                page.Paragraphs.Add(new TextFragment($"Patient ID: {facture.PatientId}"));
                page.Paragraphs.Add(new TextFragment($"Consultation ID: {facture.ConsultationId}"));
                page.Paragraphs.Add(new TextFragment($"Clinic ID: {facture.ClinicId}"));
                page.Paragraphs.Add(new TextFragment($"Date d'émission: {facture.DateEmission:dd/MM/yyyy}"));
                page.Paragraphs.Add(new TextFragment($"Montant total: {facture.MontantTotal:C}"));
                page.Paragraphs.Add(new TextFragment($"Statut: {facture.Status}"));

                using var stream = new MemoryStream();
                document.Save(stream);
                return stream.ToArray();
            });
        }
    }
}
