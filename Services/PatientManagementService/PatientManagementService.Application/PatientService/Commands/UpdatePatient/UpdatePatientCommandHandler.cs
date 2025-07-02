using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.PatientService.Commands.UpdatePatient
{
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, bool>
    {
        public readonly IPatientRepository _patientRepository;
        public readonly ILogger<UpdatePatientCommandHandler> _logger;
        private readonly IMapper _mapper;

        public UpdatePatientCommandHandler(IPatientRepository patientRepository, ILogger<UpdatePatientCommandHandler> logger, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _logger = logger;
            _mapper = mapper;
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

            _mapper.Map(request.patient, existingPatient);

            existingPatient.ModifierPatientEvent();

            await _patientRepository.UpdatePatientAsync(existingPatient);
            return true;
        }
    }
}
