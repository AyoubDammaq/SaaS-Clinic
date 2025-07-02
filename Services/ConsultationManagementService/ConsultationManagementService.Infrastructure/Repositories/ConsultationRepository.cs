using ConsultationManagementService.Data;
using ConsultationManagementService.Domain.Entities;
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

        public async Task<IEnumerable<Consultation>> GetAllConsultationsAsync(int pageNumber, int pageSize)
        {
            return await _context.Consultations
                .Include(c => c.Documents)
                .OrderByDescending(c => c.DateConsultation)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task CreateConsultationAsync(Consultation consultationDto)
        {
            if (consultationDto == null)
                throw new ArgumentNullException(nameof(consultationDto));

            await _context.Consultations.AddAsync(consultationDto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateConsultationAsync(Consultation consultationDto)
        {
            if (consultationDto == null)
                throw new ArgumentNullException(nameof(consultationDto));

            _context.Consultations.Update(consultationDto);
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

        public Task<int> CountConsultationsAsync(DateTime dateDebut, DateTime dateFin)
        {
            return _context.Consultations
                .Where(c => c.DateConsultation >= dateDebut && c.DateConsultation <= dateFin)
                .CountAsync();
        }

        public async Task<int> CountByMedecinIdsAsync(List<Guid> medecinIds)
        {
            return await _context.Consultations
                .CountAsync(c => medecinIds.Contains(c.MedecinId));
        }

        public async Task<DocumentMedical?> GetDocumentMedicalByIdAsync(Guid id)
        {
            return await _context.DocumentsMedicaux
                .Include(d => d.Consultation)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task UploadDocumentMedicalAsync(DocumentMedical documentMedicalDto)
        {
            if (documentMedicalDto == null)
                throw new ArgumentNullException(nameof(documentMedicalDto));

            // Vérifier si la consultation existe
            var consultationExists = await _context.Consultations
                .AnyAsync(c => c.Id == documentMedicalDto.ConsultationId);

            if (!consultationExists)
                throw new InvalidOperationException($"Consultation with ID {documentMedicalDto.ConsultationId} not found");

            await _context.DocumentsMedicaux.AddAsync(documentMedicalDto);
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

        public async Task<bool> ExistsAsync(Guid consultationId)
        {
            return await _context.Consultations.AnyAsync(c => c.Id == consultationId);
        }
    }
}
