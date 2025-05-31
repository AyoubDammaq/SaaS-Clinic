using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetNombreDeFactureByStatus
{
    public class GetNombreDeFactureByStatusQueryHandler : IRequestHandler<GetNombreDeFactureByStatusQuery, IEnumerable<FactureStatsDTO>>
    {
        private readonly IFactureRepository _factureRepository;
        public GetNombreDeFactureByStatusQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }
        public async Task<IEnumerable<FactureStatsDTO>> Handle(GetNombreDeFactureByStatusQuery request, CancellationToken cancellationToken)
        {
            var stats = await _factureRepository.GetNombreDeFactureByStatusAsync();

            return stats.Select(s => new FactureStatsDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            });
        }
    }
}
