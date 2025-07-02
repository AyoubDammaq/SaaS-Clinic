using AutoMapper;
using Clinic.Application.DTOs;
using Clinic.Domain.Interfaces;
using MediatR;

namespace Clinic.Application.Queries.GetStatistiquesDesCliniques
{
    public class GetStatistiquesDesCliniquesQueryHandler : IRequestHandler<GetStatistiquesDesCliniquesQuery, StatistiqueCliniqueDTO>
    {
        private readonly ICliniqueRepository _repository;
        private readonly IMapper _mapper;
        public GetStatistiquesDesCliniquesQueryHandler(ICliniqueRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<StatistiqueCliniqueDTO> Handle(GetStatistiquesDesCliniquesQuery request, CancellationToken cancellationToken)
        {
            if (request.cliniqueId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique est requis.", nameof(request.cliniqueId));
            var statistique = await _repository.GetStatistiquesDesCliniquesAsync(request.cliniqueId);
            if (statistique == null)
                throw new KeyNotFoundException($"Aucune statistique trouvée pour la clinique avec l'identifiant {request.cliniqueId}.");
            return _mapper.Map<StatistiqueCliniqueDTO>(statistique);
        }
    }
}
