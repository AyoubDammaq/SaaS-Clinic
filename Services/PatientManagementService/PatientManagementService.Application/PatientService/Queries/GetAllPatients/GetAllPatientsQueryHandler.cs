using MediatR;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.PatientService.Queries.GetAllPatients
{
    public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, IEnumerable<Patient>>
    {
        private readonly IPatientRepository _repository;

        public GetAllPatientsQueryHandler(IPatientRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Patient>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var patients = await _repository.GetAllPatientsAsync();
                return patients ?? new List<Patient>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving patients.", ex);
            }
        }
    }
}
