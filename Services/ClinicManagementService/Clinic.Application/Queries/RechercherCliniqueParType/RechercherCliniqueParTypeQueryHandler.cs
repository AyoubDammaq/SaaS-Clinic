using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.RechercherCliniqueParType
{
    public class RechercherCliniqueParTypeQueryHandler : IRequestHandler<RechercherCliniqueParTypeQuery, IEnumerable<Clinique>>
    {
        private readonly ICliniqueRepository _repository;
        public RechercherCliniqueParTypeQueryHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Clinique>> Handle(RechercherCliniqueParTypeQuery request, CancellationToken cancellationToken)
        {
            var cliniques = await _repository.GetByTypeAsync(request.type);
            return cliniques?.Where(c => c != null) ?? Enumerable.Empty<Clinique>();
        }
    }
}
