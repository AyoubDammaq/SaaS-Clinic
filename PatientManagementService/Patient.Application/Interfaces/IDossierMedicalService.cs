﻿using PatientManagementService.Models;
using PatientManagementService.DTOs;

namespace PatientManagementService.Services
{
    public interface IDossierMedicalService
    {
        Task<DossierMedical?> GetDossierMedicalByPatientIdAsync(Guid patientId);
        Task AddDossierMedicalAsync(DossierMedicalDTO dossierMedical);
        Task UpdateDossierMedicalAsync(DossierMedicalDTO dossierMedical);
        Task DeleteDossierMedicalAsync(Guid dossierMedicalId);
        Task<IEnumerable<DossierMedical>> GetAllDossiersMedicalsAsync();
        Task<DossierMedical?> GetDossierMedicalByIdAsync(Guid Id);
        Task AttacherDocumentAsync(Guid dossierMedicalId, Document document);
    }
}
