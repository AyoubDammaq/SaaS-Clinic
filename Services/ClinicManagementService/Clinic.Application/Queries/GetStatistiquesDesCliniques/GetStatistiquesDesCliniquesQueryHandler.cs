
using Clinic.Application.DTOs;
using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.GetStatistiquesDesCliniques
{
    public class GetStatistiquesDesCliniquesQueryHandler : IRequestHandler<GetStatistiquesDesCliniquesQuery, StatistiqueCliniqueDTO>
    {
        private readonly ICliniqueRepository _repository;
        public GetStatistiquesDesCliniquesQueryHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }
        public async Task<StatistiqueCliniqueDTO> Handle(GetStatistiquesDesCliniquesQuery request, CancellationToken cancellationToken)
        {
            if (request.cliniqueId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique est requis.", nameof(request.cliniqueId));
            var statistique = await _repository.GetStatistiquesDesCliniquesAsync(request.cliniqueId);
            if (statistique == null)
                throw new KeyNotFoundException($"Aucune statistique trouvée pour la clinique avec l'identifiant {request.cliniqueId}.");
            var dto = new StatistiqueCliniqueDTO
            {
                CliniqueId = statistique.CliniqueId,
                Nom = statistique.Nom,
                NombreMedecins = statistique.NombreMedecins,
                NombreConsultations = statistique.NombreConsultations,
                NombreRendezVous = statistique.NombreRendezVous,
                NombrePatients = statistique.NombrePatients
            };
            return dto;
        }
    }
}
