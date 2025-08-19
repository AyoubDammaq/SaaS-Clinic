using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetRevenusParMois
{
    public class GetRevenusParMoisQueryHandler : IRequestHandler<GetRevenusParMoisQuery, Dictionary<int, decimal>>
    {
        private readonly IFactureRepository _factureRepository;

        public GetRevenusParMoisQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }

        public async Task<Dictionary<int, decimal>> Handle(GetRevenusParMoisQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var revenusParMois = new Dictionary<int, decimal>();

            for (int mois = 1; mois <= 12; mois++)
            {
                var debutMois = new DateTime(now.Year, mois, 1);
                var finMois = debutMois.AddMonths(1);

                var revenu = await _factureRepository.GetRevenusParMoisAsync(request.ClinicId, debutMois, finMois);
                revenusParMois[mois] = revenu;
            }

            for (int mois = 1; mois <= 12; mois++)
            {
                if (!revenusParMois.ContainsKey(mois))
                    revenusParMois[mois] = 0;
            }

            return revenusParMois;
        }
    }
}
