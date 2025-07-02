using AutoMapper;
using Clinic.Application.DTOs;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.GetNombreNouvellesCliniquesParMois
{
    public class GetNombreNouvellesCliniquesParMoisQueryHandler : IRequestHandler<GetNombreNouvellesCliniquesParMoisQuery, IEnumerable<StatistiqueDTO>>
    {
        private readonly ICliniqueRepository _repository;
        private readonly IMapper _mapper;
        public GetNombreNouvellesCliniquesParMoisQueryHandler(ICliniqueRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<StatistiqueDTO>> Handle(GetNombreNouvellesCliniquesParMoisQuery request, CancellationToken cancellationToken)
        {
            var statistiques = await _repository.GetNombreNouvellesCliniquesParMoisAsync();
            if (statistiques == null || !statistiques.Any())
                throw new InvalidOperationException("Aucune statistique trouvée.");
            return _mapper.Map<IEnumerable<StatistiqueDTO>>(statistiques);
        }
    }
}
