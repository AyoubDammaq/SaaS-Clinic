using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;
using MediatR;
using AutoMapper;
using PatientManagementService.Application.DTOs;

namespace PatientManagementService.Application.PatientService.Commands.AddPatient
{
    public class AddPatientCommandHandler : IRequestHandler<AddPatientCommand, PatientDTO>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<AddPatientCommandHandler> _logger;
        private readonly IMapper _mapper;

        public AddPatientCommandHandler(IPatientRepository patientRepository, ILogger<AddPatientCommandHandler> logger, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PatientDTO> Handle(AddPatientCommand request, CancellationToken cancellationToken)
        {
            if (request.patient == null)
            {
                _logger.LogWarning("Tentative d'ajouter un patient null.");
                throw new ArgumentNullException(nameof(request.patient), "PatientDTO ne peut pas être null.");
            }

            // 1. Mapper le DTO vers l'entité
            var newPatient = _mapper.Map<Patient>(request.patient);
            newPatient.Id = Guid.NewGuid();
            newPatient.DateCreation = DateTime.UtcNow;

            // 2. Ajouter un éventuel domaine event
            newPatient.AjouterPatientEvent();

            // 3. Persister l'entité
            await _patientRepository.AddPatientAsync(newPatient);

            // 4. Retourner un PatientDTO complet
            var patientDto = _mapper.Map<PatientDTO>(newPatient);
            return patientDto;
        }
    }
}
