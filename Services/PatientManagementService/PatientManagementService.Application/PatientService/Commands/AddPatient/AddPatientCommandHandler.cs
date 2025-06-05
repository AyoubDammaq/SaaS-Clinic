using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;
using MediatR;

namespace PatientManagementService.Application.PatientService.Commands.AddPatient
{
    public class AddPatientCommandHandler : IRequestHandler<AddPatientCommand, bool>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<AddPatientCommandHandler> _logger;

        public AddPatientCommandHandler(IPatientRepository patientRepository, ILogger<AddPatientCommandHandler> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(AddPatientCommand request, CancellationToken cancellationToken)
        {
            if (request.patient == null)
            {
                _logger.LogWarning("Tentative d'ajouter un patient null.");
                return false;
            }

            var newPatient = new Patient
            {
                Id = Guid.NewGuid(),
                Nom = request.patient.Nom,
                Prenom = request.patient.Prenom,
                DateNaissance = request.patient.DateNaissance,
                Sexe = request.patient.Sexe,
                Adresse = request.patient.Adresse,
                Telephone = request.patient.Telephone,
                Email = request.patient.Email
            };

            newPatient.AjouterPatientEvent();

            await _patientRepository.AddPatientAsync(newPatient);
            return true;
        }
    }
}
