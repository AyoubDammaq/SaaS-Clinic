using Clinic.Application.DTOs;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.GetNombreNouvellesCliniquesParMois
{
    public class GetNombreNouvellesCliniquesParMoisQueryHandler : IRequestHandler<GetNombreNouvellesCliniquesParMoisQuery, IEnumerable<StatistiqueDTO>>
    {
        private readonly ICliniqueRepository _repository;
        public GetNombreNouvellesCliniquesParMoisQueryHandler(ICliniqueRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<StatistiqueDTO>> Handle(GetNombreNouvellesCliniquesParMoisQuery request, CancellationToken cancellationToken)
        {
            var statistiques = await _repository.GetNombreNouvellesCliniquesParMoisAsync();
            if (statistiques == null || !statistiques.Any())
                throw new InvalidOperationException("Aucune statistique trouvée.");
            return statistiques.Select(s => new StatistiqueDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            }).ToList();
        }
    }
}
