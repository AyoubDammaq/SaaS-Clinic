using MediatR;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Commands.AddDossierMedical
{
    public class AddDossierMedicalCommandHandler : IRequestHandler<AddDossierMedicalCommand>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;
        private readonly IPatientRepository _patientRepository;

        public AddDossierMedicalCommandHandler(IDossierMedicalRepository repository, IPatientRepository patientRepository)
        {
            _dossierMedicalRepository = repository;
            _patientRepository = patientRepository;
        }

        public async Task Handle(AddDossierMedicalCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(request.dossierMedical.PatientId)
                ?? throw new Exception("Patient not found");

            if (patient.DossierMedicalId != null)
            {
                throw new InvalidOperationException("Patient already has a dossier médical.");
            }

            var dossierMedicalEntity = new DossierMedical
            {
                Id = request.dossierMedical.Id,
                PatientId = request.dossierMedical.PatientId,
                Allergies = request.dossierMedical.Allergies,
                MaladiesChroniques = request.dossierMedical.MaladiesChroniques,
                MedicamentsActuels = request.dossierMedical.MedicamentsActuels,
                AntécédentsFamiliaux = request.dossierMedical.AntécédentsFamiliaux,
                AntécédentsPersonnels = request.dossierMedical.AntécédentsPersonnels,
                GroupeSanguin = request.dossierMedical.GroupeSanguin,
                DateCreation = DateTime.UtcNow
            };

            patient.DossierMedicalId = request.dossierMedical.Id;

            dossierMedicalEntity.CreerDossierMedicalEvent();
            patient.ModifierPatientEvent();

            await _dossierMedicalRepository.AddDossierMedicalAsync(dossierMedicalEntity);
            await _patientRepository.UpdatePatientAsync(patient);
        }
    }
}
