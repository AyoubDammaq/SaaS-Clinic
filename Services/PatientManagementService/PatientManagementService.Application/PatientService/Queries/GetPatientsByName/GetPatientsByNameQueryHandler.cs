using MediatR;
using Microsoft.Extensions.Logging;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.PatientService.Queries.GetPatientsByName
{
    public class GetPatientsByNameQueryHandler : IRequestHandler<GetPatientsByNameQuery, IEnumerable<Patient>>
    {
        private readonly IPatientRepository _repository;
        private readonly ILogger<GetPatientsByNameQueryHandler> _logger;

        public GetPatientsByNameQueryHandler(IPatientRepository repository, ILogger<GetPatientsByNameQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Patient>> Handle(GetPatientsByNameQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.name) || string.IsNullOrWhiteSpace(request.lastname))
            {
                _logger.LogWarning("Requête invalide : nom ou prénom manquant.");
                return Enumerable.Empty<Patient>();
            }

            return await _repository.GetPatientsByNameAsync(request.name, request.lastname) ?? Enumerable.Empty<Patient>();
        }
    }
}
