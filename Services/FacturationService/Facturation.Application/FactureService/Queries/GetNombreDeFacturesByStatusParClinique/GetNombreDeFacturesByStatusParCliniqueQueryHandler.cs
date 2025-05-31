using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusParClinique
{
    public class GetNombreDeFacturesByStatusParCliniqueQueryHandler : IRequestHandler<GetNombreDeFacturesByStatusParCliniqueQuery, IEnumerable<FactureStatsDTO>>
    {
        private readonly IFactureRepository _factureRepository;
        public GetNombreDeFacturesByStatusParCliniqueQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }
        public async Task<IEnumerable<FactureStatsDTO>> Handle(GetNombreDeFacturesByStatusParCliniqueQuery request, CancellationToken cancellationToken)
        {
            var stats = await _factureRepository.GetNombreDeFacturesByStatusParCliniqueAsync();

            return stats.Select(s => new FactureStatsDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            });
        }
    }
}
