using MediatR;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.PatientService.Queries.GetStatistiques
{
    public class GetStatistiquesQueryHandler : IRequestHandler<GetStatistiquesQuery, int>
    {
        private readonly IPatientRepository _repository;

        public GetStatistiquesQueryHandler(IPatientRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(GetStatistiquesQuery request, CancellationToken cancellationToken)
        {
            if (request.dateDebut > request.dateFin)
            {
                throw new ArgumentException("La date de début doit être antérieure à la date de fin.");
            }
            return await _repository.GetStatistiquesAsync(request.dateDebut, request.dateFin);
        }
    }
}
