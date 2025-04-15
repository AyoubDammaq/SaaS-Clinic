using ConsultationManagementService.Data;
using ConsultationManagementService.DTOs;
using ConsultationManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultationManagementService.Repositories
{
    public class ConsultationRepository : IConsultationRepository
    {
        private readonly ConsultationDbContext _context;

        public ConsultationRepository(ConsultationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Consultation?> GetConsultationByIdAsync(Guid id)
        {
            return await _context.Consultations
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Consultation>> GetAllConsultationsAsync()
        {
            return await _context.Consultations
                .Include(c => c.Documents)
                .ToListAsync();
        }

        public async Task CreateConsultationAsync(ConsultationDTO consultationDto)
        {
            if (consultationDto == null)
                throw new ArgumentNullException(nameof(consultationDto));

            var consultation = new Consultation
            {
                Id = consultationDto.Id,
                DateConsultation = consultationDto.DateConsultation,
                PatientId = consultationDto.PatientId,
                MedecinId = consultationDto.MedecinId,
                Diagnostic = consultationDto.Diagnostic,
                Notes = consultationDto.Notes
            };

            await _context.Consultations.AddAsync(consultation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateConsultationAsync(ConsultationDTO consultationDto)
        {
            if (consultationDto == null)
                throw new ArgumentNullException(nameof(consultationDto));

            var consultation = await _context.Consultations
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == consultationDto.Id);

            if (consultation == null)
                throw new InvalidOperationException($"Consultation with ID {consultationDto.Id} not found");

            consultation.DateConsultation = consultationDto.DateConsultation;
            consultation.PatientId = consultationDto.PatientId;
            consultation.MedecinId = consultationDto.MedecinId;
            consultation.Diagnostic = consultationDto.Diagnostic;
            consultation.Notes = consultationDto.Notes;

            _context.Consultations.Update(consultation);
            await _context.SaveChangesAsync();
        }  

        public async Task<bool> DeleteConsultationAsync(Guid id)
        {
            var consultation = await _context.Consultations
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consultation == null)
                return false;

            _context.Consultations.Remove(consultation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Consultation>> GetConsultationsByPatientIdAsync(Guid patientId)
        {
            return await _context.Consultations
                .Include(c => c.Documents)
                .Where(c => c.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Consultation>> GetConsultationsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Consultations
                .Include(c => c.Documents)
                .Where(c => c.MedecinId == doctorId)
                .ToListAsync();
        }

        public async Task<DocumentMedical?> GetDocumentMedicalByIdAsync(Guid id)
        {
            return await _context.DocumentsMedicaux
                .Include(d => d.Consultation)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task UploadDocumentMedicalAsync(DocumentMedicalDTO documentMedicalDto)
        {
            if (documentMedicalDto == null)
                throw new ArgumentNullException(nameof(documentMedicalDto));

            // Vérifier si la consultation existe
            var consultationExists = await _context.Consultations
                .AnyAsync(c => c.Id == documentMedicalDto.ConsultationId);

            if (!consultationExists)
                throw new InvalidOperationException($"Consultation with ID {documentMedicalDto.ConsultationId} not found");

            var documentMedical = new DocumentMedical
            {
                Id = documentMedicalDto.Id,
                ConsultationId = documentMedicalDto.ConsultationId,
                Type = documentMedicalDto.Type,
                FichierURL = documentMedicalDto.FichierURL,
                DateAjout = DateTime.Now
            };

            await _context.DocumentsMedicaux.AddAsync(documentMedical);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteDocumentMedicalAsync(Guid id)
        {
            var document = await _context.DocumentsMedicaux
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
                return false;

            _context.DocumentsMedicaux.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
