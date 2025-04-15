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
            _context = context;
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
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
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
            var consultation = await _context.Consultations
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == consultationDto.Id);
            if (consultation != null)
            {
                consultation.DateConsultation = consultationDto.DateConsultation;
                consultation.PatientId = consultationDto.PatientId;
                consultation.MedecinId = consultationDto.MedecinId;
                consultation.Diagnostic = consultationDto.Diagnostic;
                consultation.Notes = consultationDto.Notes;

                _context.Consultations.Update(consultation);
                await _context.SaveChangesAsync();
            }
        }  

        public async Task<bool> DeleteConsultationAsync(Guid id)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
                return false;
            _context.Consultations.Remove(consultation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DocumentMedical?> GetDocumentMedicalByIdAsync(Guid id)
        {
            return await _context.DocumentsMedicaux.FindAsync(id);
            
        }

        public async Task UploadDocumentMedicalAsync(DocumentMedicalDTO documentMedicalDto)
        {
            var documentMedical = new DocumentMedical
            {
                Id = Guid.NewGuid(),
                ConsultationId = documentMedicalDto.ConsultationId,
                Type = documentMedicalDto.Type,
                FichierURL = documentMedicalDto.FichierURL,
                DateAjout = documentMedicalDto.DateAjout
            };

            await _context.DocumentsMedicaux.AddAsync(documentMedical);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteDocumentMedicalAsync(Guid id)
        {
            var document = await _context.DocumentsMedicaux.FindAsync(id);
            if (document == null)
                return false;
            _context.DocumentsMedicaux.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
