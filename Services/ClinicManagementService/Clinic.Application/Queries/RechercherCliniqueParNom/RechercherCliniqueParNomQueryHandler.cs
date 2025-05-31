using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.RechercherCliniqueParNom
{
    public class RechercherCliniqueParNomQueryHandler : IRequestHandler<RechercherCliniqueParNomQuery, IEnumerable<Clinique>>
    {
        private readonly ICliniqueRepository _repository;

        public RechercherCliniqueParNomQueryHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Clinique>> Handle(RechercherCliniqueParNomQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Nom))
                throw new ArgumentException("Le nom de la clinique est requis.", nameof(request.Nom));

            var cliniques = await _repository.GetByNameAsync(request.Nom);
            return cliniques?.Where(c => c != null) ?? Enumerable.Empty<Clinique>();
        }
    }
}
