using MediatR;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.PatientService.Commands.LinkUserToPatient
{
    public class LinkUserToPatientCommandHandler : IRequestHandler<LinkUserToPatientCommand, bool>
    {
        private readonly IPatientRepository _patientRepository;
        public LinkUserToPatientCommandHandler(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }
        public async Task<bool> Handle(LinkUserToPatientCommand request, CancellationToken cancellationToken)
        {
            return await _patientRepository.LinkUserToPatientEntityAsync(request.LinkDto.PatientId, request.LinkDto.UserId);
        }
    }
}
