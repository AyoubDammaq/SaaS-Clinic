using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.RechercherCliniqueParAdresse
{
    public class RechercherCliniqueParAdresseQueryHandler : IRequestHandler<RechercherCliniqueParAdresseQuery, IEnumerable<Clinique>>
    {
        private readonly ICliniqueRepository _repository;

        public RechercherCliniqueParAdresseQueryHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Clinique>> Handle(RechercherCliniqueParAdresseQuery request, CancellationToken cancellationToken)
        {
            // Si l'adresse est vide, on retourne toutes les cliniques
            if (string.IsNullOrWhiteSpace(request.Adresse))
                return await _repository.GetAllAsync();

            var cliniques = await _repository.GetByAddressAsync(request.Adresse);
            return cliniques?.Where(c => c != null) ?? Enumerable.Empty<Clinique>();
        }
    }
}
