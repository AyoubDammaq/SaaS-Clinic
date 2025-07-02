using Facturation.Application.DTOs;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetStatistiquesFacturesParPeriode
{
    public class GetStatistiquesFacturesParPeriodeQueryHandler
    : IRequestHandler<GetStatistiquesFacturesParPeriodeQuery, StatistiquesFacturesDto>
    {
        private readonly IFactureRepository _factureRepository;

        public GetStatistiquesFacturesParPeriodeQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }

        public async Task<StatistiquesFacturesDto> Handle(GetStatistiquesFacturesParPeriodeQuery request, CancellationToken cancellationToken)
        {
            var factures = await _factureRepository.GetFacturesParPeriode(request.DateDebut, request.DateFin);

            var stats = new StatistiquesFacturesDto
            {
                NombreTotal = factures.Count,
                NombrePayees = factures.Count(f => f.Status == FactureStatus.PAYEE),
                NombreImpayees = factures.Count(f => f.Status == FactureStatus.IMPAYEE),
                NombrePartiellementPayees = factures.Count(f => f.Status == FactureStatus.PARTIELLEMENT_PAYEE),
                MontantTotal = factures.Sum(f => f.MontantTotal),
                MontantTotalPaye = factures.Sum(f => f.MontantPaye),
                NombreParClinique = factures
                    .GroupBy(f => f.ClinicId)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return stats;
        }
    }
}
