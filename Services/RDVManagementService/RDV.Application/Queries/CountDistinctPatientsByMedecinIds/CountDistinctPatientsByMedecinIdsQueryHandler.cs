using MediatR;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.CountDistinctPatientsByMedecinIds
{
    public class CountDistinctPatientsByMedecinIdsQueryHandler : IRequestHandler<CountDistinctPatientsByMedecinIdsQuery, int>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public CountDistinctPatientsByMedecinIdsQueryHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task<int> Handle(CountDistinctPatientsByMedecinIdsQuery request, CancellationToken cancellationToken)
        {
            if (request.medecinIds == null || !request.medecinIds.Any())
            {
                throw new ArgumentException("La liste des identifiants de médecins ne peut pas être vide.", nameof(request.medecinIds));
            }
            return await _rendezVousRepository.CountDistinctPatientsByMedecinIdsAsync(request.medecinIds);
        }
    }
}
