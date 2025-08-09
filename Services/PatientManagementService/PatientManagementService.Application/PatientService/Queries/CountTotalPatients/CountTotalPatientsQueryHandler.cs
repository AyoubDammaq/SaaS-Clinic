using MediatR;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.PatientService.Queries.CountTotalPatients
{
    public class CountTotalPatientsQueryHandler : IRequestHandler<CountTotalPatientsQuery, int>
    {
        private readonly IPatientRepository _patientRepository;
        public CountTotalPatientsQueryHandler(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }
        public async Task<int> Handle(CountTotalPatientsQuery request, CancellationToken cancellationToken)
        {
            return await _patientRepository.CountTotalPatientsAsync();
        }
    }
}
