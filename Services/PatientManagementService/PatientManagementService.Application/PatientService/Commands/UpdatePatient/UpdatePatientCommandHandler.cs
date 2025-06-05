using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.PatientService.Commands.UpdatePatient
{
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, bool>
    {
        public readonly IPatientRepository _patientRepository;
        public readonly ILogger<UpdatePatientCommandHandler> _logger;

        public UpdatePatientCommandHandler(IPatientRepository patientRepository, ILogger<UpdatePatientCommandHandler> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            if (request.patient == null)
            {
                _logger.LogWarning("Mise à jour échouée : patient null.");
                return false;
            }

            var existingPatient = await _patientRepository.GetPatientByIdAsync(request.patient.Id);
            if (existingPatient == null)
            {
                _logger.LogWarning($"Patient introuvable pour mise à jour (ID : {request.patient.Id}).");
                return false;
            }

            existingPatient.Nom = request.patient.Nom;
            existingPatient.Prenom = request.patient.Prenom;
            existingPatient.DateNaissance = request.patient.DateNaissance;
            existingPatient.Sexe = request.patient.Sexe;
            existingPatient.Adresse = request.patient.Adresse;
            existingPatient.Telephone = request.patient.Telephone;
            existingPatient.Email = request.patient.Email;

            existingPatient.ModifierPatientEvent();

            await _patientRepository.UpdatePatientAsync(existingPatient);
            return true;
        }
    }
}
