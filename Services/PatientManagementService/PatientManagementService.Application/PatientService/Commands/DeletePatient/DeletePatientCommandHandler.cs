using Microsoft.Extensions.Logging;
using PatientManagementService.Application.PatientService.Commands.AddPatient;
using PatientManagementService.Domain.Interfaces;
using MediatR;

namespace PatientManagementService.Application.PatientService.Commands.DeletePatient
{
    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, bool>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<AddPatientCommandHandler> _logger;
        public DeletePatientCommandHandler(IPatientRepository patientRepository, ILogger<AddPatientCommandHandler> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }
        public async Task<bool> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var existingPatient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
            if (existingPatient == null)
            {
                _logger.LogWarning($"Patient introuvable pour suppression (ID : {request.PatientId}).");
                return false;
            }

            await _patientRepository.DeletePatientAsync(request.PatientId);
            return true;
        }
    }
}
