using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;
using MediatR;
using AutoMapper;

namespace PatientManagementService.Application.PatientService.Commands.AddPatient
{
    public class AddPatientCommandHandler : IRequestHandler<AddPatientCommand, bool>
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

        public async Task<bool> Handle(AddPatientCommand request, CancellationToken cancellationToken)
        {
            if (request.patient == null)
            {
                _logger.LogWarning("Tentative d'ajouter un patient null.");
                return false;
            }

            var newPatient = _mapper.Map<Patient>(request.patient);
            newPatient.Id = Guid.NewGuid();

            newPatient.AjouterPatientEvent();

            await _patientRepository.AddPatientAsync(newPatient);
            return true;
        }
    }
}
