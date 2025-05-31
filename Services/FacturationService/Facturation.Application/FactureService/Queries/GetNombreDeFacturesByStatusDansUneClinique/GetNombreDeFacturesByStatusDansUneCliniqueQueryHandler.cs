using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusDansUneClinique
{
    public class GetNombreDeFacturesByStatusDansUneCliniqueQueryHandler : IRequestHandler<GetNombreDeFacturesByStatusDansUneCliniqueQuery, IEnumerable<FactureStatsDTO>>
    {
        private readonly IFactureRepository _factureRepository;
        public GetNombreDeFacturesByStatusDansUneCliniqueQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }
        public async Task<IEnumerable<FactureStatsDTO>> Handle(GetNombreDeFacturesByStatusDansUneCliniqueQuery request, CancellationToken cancellationToken)
        {
            if (request.cliniqueId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique ne peut pas être vide.", nameof(request.cliniqueId));
            var stats = await _factureRepository.GetNombreDeFacturesByStatusDansUneCliniqueAsync(request.cliniqueId);

            return stats.Select(s => new FactureStatsDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            });
        }
    }
}
