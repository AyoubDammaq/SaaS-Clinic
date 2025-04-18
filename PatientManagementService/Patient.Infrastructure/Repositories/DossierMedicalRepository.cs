﻿using Microsoft.EntityFrameworkCore;
using PatientManagementService.Data;
using PatientManagementService.Models;

namespace PatientManagementService.Repositories
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
            var dossierMedical = await _context.DossiersMedicaux.FindAsync(dossierMedicalId);
            if (dossierMedical != null)
            {
                _context.DossiersMedicaux.Remove(dossierMedical);
                await _context.SaveChangesAsync();
            }
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
    }
}
