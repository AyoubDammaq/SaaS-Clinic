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

        public async Task<IEnumerable<Consultation>> GetConsultationsByClinicIdAsync(Guid clinicId)
        {
            return await _context.Consultations
                .Include(c => c.Documents)
                .Where(c => c.ClinicId == clinicId)
                .ToListAsync();
        }

        public async Task<int> CountNouveauxPatientsByDoctorAsync(Guid medecinId, DateTime startDate, DateTime endDate)
        {
            // 1. Récupérer toutes les consultations de ce médecin
            var consultations = await _context.Consultations
                .Where(c => c.MedecinId == medecinId)
                .OrderBy(c => c.DateConsultation)
                .ToListAsync();

            // 2. Grouper par patient et ne garder que la première consultation pour chaque
            var firstConsultations = consultations
                .GroupBy(c => c.PatientId)
                .Select(g => g.First()) // La première consultation chronologiquement
                .ToList();

            // 3. Compter combien ont leur première consultation dans l’intervalle donné
            var nouveauxPatientsCount = firstConsultations
                .Count(c => c.DateConsultation >= startDate && c.DateConsultation <= endDate);

            return nouveauxPatientsCount;
        }

        public async Task<int> CountNouveauxPatientsByClinicAsync(Guid clinicId, DateTime startDate, DateTime endDate)
        {
            // 1. Récupérer toutes les consultations de cette clinique
            var consultations = await _context.Consultations
                .Where(c => c.ClinicId == clinicId)
                .OrderBy(c => c.DateConsultation)
                .ToListAsync();
            // 2. Grouper par patient et ne garder que la première consultation pour chaque
            var firstConsultations = consultations
                .GroupBy(c => c.PatientId)
                .Select(g => g.First()) // La première consultation chronologiquement
                .ToList();
            // 3. Compter combien ont leur première consultation dans l’intervalle donné
            var nouveauxPatientsCount = firstConsultations
                .Count(c => c.DateConsultation >= startDate && c.DateConsultation <= endDate);
            return nouveauxPatientsCount;
        }

        public async Task<int> CountConsultationByDateAsync(Guid? cliniqueId, Guid? medecinId, Guid? patientId, DateTime dateDebut, DateTime dateFin)
        {
            var query = _context.Consultations.AsQueryable();

            query = query.Where(c => c.DateConsultation >= dateDebut && c.DateConsultation <= dateFin);

            if (cliniqueId.HasValue)
                query = query.Where(c => c.ClinicId == cliniqueId.Value);

            if (medecinId.HasValue)
                query = query.Where(c => c.MedecinId == medecinId.Value);

            if (patientId.HasValue)
                query = query.Where(c => c.PatientId == patientId.Value);

            return await query.CountAsync();
        }

        public async Task<int> CountConsultationByPatient(Guid patientId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Consultations.AsQueryable();
            query = query.Where(c => c.PatientId == patientId);
            if (startDate.HasValue)
                query = query.Where(c => c.DateConsultation >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(c => c.DateConsultation <= endDate.Value);
            return await query.CountAsync();
        }

        public async Task<int> CountConsultationByDoctor(Guid medecinId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Consultations.AsQueryable();
            query = query.Where(c => c.MedecinId == medecinId);
            if (startDate.HasValue)
                query = query.Where(c => c.DateConsultation >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(c => c.DateConsultation <= endDate.Value);
            return await query.CountAsync();
        }

        public async Task<int> CountConsultationByClinic(Guid clinicId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Consultations.AsQueryable();
            query = query.Where(c => c.ClinicId == clinicId);
            if (startDate.HasValue)
                query = query.Where(c => c.DateConsultation >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(c => c.DateConsultation <= endDate.Value);
            return await query.CountAsync();
        }
    }
}
