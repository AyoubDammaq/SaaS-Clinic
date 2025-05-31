using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetNombreDeFactureParClinique
{
    public class GetNombreDeFactureParCliniqueQueryHandler : IRequestHandler<GetNombreDeFactureParCliniqueQuery, IEnumerable<FactureStatsDTO>>
    {
        private readonly IFactureRepository _factureRepository;
        public GetNombreDeFactureParCliniqueQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }
        public async Task<IEnumerable<FactureStatsDTO>> Handle(GetNombreDeFactureParCliniqueQuery request, CancellationToken cancellationToken)
        {
            var stats = await _factureRepository.GetNombreDeFactureParCliniqueAsync();

            return stats.Select(s => new FactureStatsDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            });
        }
    }
}
