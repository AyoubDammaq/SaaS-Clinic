using Microsoft.EntityFrameworkCore;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;
using PatientManagementService.Infrastructure.Data;


namespace PatientManagementService.Infrastructure.Repositories
{
    public class DossierMedicalRepository : IDossierMedicalRepository
    {
        private readonly PatientDbContext _context;
        public DossierMedicalRepository(PatientDbContext context)
        {
            _context = context;
        }
        public async Task<DossierMedical?> GetDossierMedicalByPatientIdAsync(Guid patientId)
        {
            return await _context.DossiersMedicaux
                .Include(dm => dm.Patient)
                .Include(dm => dm.Documents)
                .FirstOrDefaultAsync(dm => dm.PatientId == patientId);
        }
        public async Task AddDossierMedicalAsync(DossierMedical dossierMedical)
        {
            await _context.DossiersMedicaux.AddAsync(dossierMedical);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateDossierMedicalAsync(DossierMedical dossierMedical)
        {
            var dossierMedicalUpdated = await _context.DossiersMedicaux.FindAsync(dossierMedical.Id);
            if (dossierMedicalUpdated != null)
            {
                _context.DossiersMedicaux.Update(dossierMedicalUpdated);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteDossierMedicalAsync(Guid dossierMedicalId)
        {
            var dossierMedical = await _context.DossiersMedicaux
                .AsTracking() 
                .FirstOrDefaultAsync(dm => dm.Id == dossierMedicalId);

            if (dossierMedical == null)
                return;

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == dossierMedical.PatientId);

            if (patient != null)
            {
                patient.DossierMedicalId = null;
                patient.DossierMedical = null;
            }

            _context.DossiersMedicaux.Remove(dossierMedical);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DossierMedical>> GetAllDossiersMedicalsAsync()
        {
            return await _context.DossiersMedicaux
                .Include(dm => dm.Patient)
                .Include(dm => dm.Documents)
                .ToListAsync();
        }

        public async Task<DossierMedical?> GetDossierMedicalByIdAsync(Guid Id)
        {
            return await _context.DossiersMedicaux.FindAsync(Id);
        }

        public async Task AttacherDocumentAsync(Guid dossierMedicalId, Document document)
        {
            var dossierMedical = await _context.DossiersMedicaux.Include(dm => dm.Documents).FirstOrDefaultAsync(dm => dm.Id == dossierMedicalId);
            if (dossierMedical != null)
            {
                document.DossierMedicalId = dossierMedicalId;
                _context.Documents.Add(document);
                await _context.SaveChangesAsync();
            }
        }

        // ✅ NEW: Get a document by its ID
        public async Task<Document?> GetDocumentByIdAsync(Guid documentId)
        {
            return await _context.Documents
                .FirstOrDefaultAsync(doc => doc.Id == documentId);
        }

        // ✅ NEW: Remove a document by its ID
        public async Task RemoveDocumentAsync(Guid documentId)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(doc => doc.Id == documentId);

            if (document == null)
            {
                return;
            }
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}
