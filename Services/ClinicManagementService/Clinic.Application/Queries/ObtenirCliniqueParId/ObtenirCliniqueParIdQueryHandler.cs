using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.ObtenirCliniqueParId
{
    public class ObtenirCliniqueParIdQueryHandler : IRequestHandler<ObtenirCliniqueParIdQuery, Clinique>
    {
        private readonly ICliniqueRepository _repository;

        public ObtenirCliniqueParIdQueryHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }

        public async Task<Clinique> Handle(ObtenirCliniqueParIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique est requis.", nameof(request.Id));

            var clinique = await _repository.GetByIdAsync(request.Id);
            if (clinique == null)
                throw new KeyNotFoundException($"Aucune clinique trouvée avec l'identifiant {request.Id}.");

            return clinique;
        }
    }
}
