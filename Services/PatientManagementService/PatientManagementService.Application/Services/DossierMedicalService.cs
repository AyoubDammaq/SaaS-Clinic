using PatientManagementService.Application.DTOs;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.Services
{
    public class DossierMedicalService : IDossierMedicalService
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;
        private readonly IPatientRepository _patientRepository;
        public DossierMedicalService(IDossierMedicalRepository dossierMedicalRepository, IPatientRepository patientRepository)
        {
            _dossierMedicalRepository = dossierMedicalRepository;
            _patientRepository = patientRepository;
        }

        public async Task<DossierMedical?> GetDossierMedicalByPatientIdAsync(Guid patientId)
        {
            return await _dossierMedicalRepository.GetDossierMedicalByPatientIdAsync(patientId);
        }

        public async Task AddDossierMedicalAsync(DossierMedicalDTO dossierMedical)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(dossierMedical.PatientId)
                ?? throw new Exception("Patient not found");

            if (patient.DossierMedicalId != null)
            {
                throw new InvalidOperationException("Patient already has a dossier médical.");
            }

            var dossierMedicalEntity = new DossierMedical
            {
                Id = dossierMedical.Id,
                PatientId = dossierMedical.PatientId,
                Allergies = dossierMedical.Allergies,
                MaladiesChroniques = dossierMedical.MaladiesChroniques,
                MedicamentsActuels = dossierMedical.MedicamentsActuels,
                AntécédentsFamiliaux = dossierMedical.AntécédentsFamiliaux,
                AntécédentsPersonnels = dossierMedical.AntécédentsPersonnels,
                GroupeSanguin = dossierMedical.GroupeSanguin,
                DateCreation = DateTime.UtcNow
            };

            patient.DossierMedicalId = dossierMedical.Id;
            await _dossierMedicalRepository.AddDossierMedicalAsync(dossierMedicalEntity);
            await _patientRepository.UpdatePatientAsync(patient);
        }

        public async Task UpdateDossierMedicalAsync(DossierMedicalDTO dossierMedical)
        {
            var existingDossier = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(dossierMedical.Id)
                ?? throw new Exception("Dossier médical not found");

            existingDossier.Allergies = dossierMedical.Allergies;
            existingDossier.MaladiesChroniques = dossierMedical.MaladiesChroniques;
            existingDossier.MedicamentsActuels = dossierMedical.MedicamentsActuels;
            existingDossier.AntécédentsFamiliaux = dossierMedical.AntécédentsFamiliaux;
            existingDossier.AntécédentsPersonnels = dossierMedical.AntécédentsPersonnels;
            existingDossier.GroupeSanguin = dossierMedical.GroupeSanguin;

            await _dossierMedicalRepository.UpdateDossierMedicalAsync(existingDossier);
        }

        public async Task DeleteDossierMedicalAsync(Guid dossierMedicalId)
        {
            var dossierMedical = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(dossierMedicalId) ?? throw new Exception("Dossier médical not found");
            await _dossierMedicalRepository.DeleteDossierMedicalAsync(dossierMedicalId);
        }

        public async Task<IEnumerable<DossierMedical>> GetAllDossiersMedicalsAsync()
        {
            return await _dossierMedicalRepository.GetAllDossiersMedicalsAsync();
        }

        public async Task<DossierMedical?> GetDossierMedicalByIdAsync(Guid Id)
        {
            var dossierMedical = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(Id) ?? throw new Exception("Dossier médical not found");
            return dossierMedical;
        }

        public async Task AttacherDocumentAsync(Guid dossierMedicalId, DocumentDTO document)
        {
            var dossierMedical = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(dossierMedicalId) ?? throw new InvalidOperationException("Dossier médical not found.");
            var newDucument = new Document
            {
                Nom = document.Nom,
                Url = document.Url,
                Type = document.Type
            };
            await _dossierMedicalRepository.AttacherDocumentAsync(dossierMedicalId, newDucument);
        }
        public async Task<Document?> GetDocumentByIdAsync(Guid documentId)
        {
            var document = await _dossierMedicalRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
                throw new Exception("Document not found");

            return document;
        }

        public async Task RemoveDocumentAsync(Guid documentId)
        {
            var document = await _dossierMedicalRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
                throw new Exception("Document not found");

            await _dossierMedicalRepository.RemoveDocumentAsync(documentId);
        }

    }
}
