using MediatR;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.CountByMedecinIds
{
    public class CountByMedecinIdsQueryHandler : IRequestHandler<CountByMedecinIdsQuery, int>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public CountByMedecinIdsQueryHandler(IRendezVousRepository repository)
        {
            _rendezVousRepository = repository;
        }
        public async Task<int> Handle(CountByMedecinIdsQuery request, CancellationToken cancellationToken)
        {
            if (request.medecinIds == null || !request.medecinIds.Any())
            {
                throw new ArgumentException("La liste des identifiants de médecins ne peut pas être vide.", nameof(request.medecinIds));
            }
            return await _rendezVousRepository.CountByMedecinIdsAsync(request.medecinIds);
        }
    }
}
