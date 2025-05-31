using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.RechercherCliniqueParStatut
{
    public class RechercherCliniqueParStatutQueryHandler : IRequestHandler<RechercherCliniqueParStatusQuery, IEnumerable<Clinique>>
    {
        private readonly ICliniqueRepository _repository;
        public RechercherCliniqueParStatutQueryHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Clinique>> Handle(RechercherCliniqueParStatusQuery request, CancellationToken cancellationToken)
        {
            var cliniques = await _repository.GetByStatusAsync(request.statut);
            return cliniques?.Where(c => c != null) ?? Enumerable.Empty<Clinique>();
        }
    }
}
