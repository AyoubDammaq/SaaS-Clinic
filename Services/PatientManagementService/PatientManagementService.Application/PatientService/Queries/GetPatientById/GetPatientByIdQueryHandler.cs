using MediatR;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.PatientService.Queries.GetPatientById
{
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, Patient?>
    {
        private readonly IPatientRepository _repository;
        public GetPatientByIdQueryHandler(IPatientRepository repository)
        {
            _repository = repository;
        }
        public async Task<Patient?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetPatientByIdAsync(request.id);
        }
    }
}
