using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetRevenusMensuelTrend
{
    public class GetRevenusMensuelTrendQueryHandler : IRequestHandler<GetRevenusMensuelTrendQuery, RevenusMensuelTrendDto>
    {
        private readonly IFactureRepository _factureRepository;
        public GetRevenusMensuelTrendQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository ?? throw new ArgumentNullException(nameof(factureRepository));
        }
        public async Task<RevenusMensuelTrendDto> Handle(GetRevenusMensuelTrendQuery request, CancellationToken cancellationToken)
        {
            if (request.clinicId == Guid.Empty)
            {
                throw new ArgumentException("Clinic ID cannot be empty.", nameof(request.clinicId));
            }

            var (current, previous) = await _factureRepository.GetRevenusMensuelTrendAsync(request.clinicId);

            var percentageChange = previous == 0 ? 100 : ((double)(current - previous) / (double)previous) * 100;
            var isPositive = current >= previous;

            return new RevenusMensuelTrendDto
            {
                Current = current,
                Previous = previous,
                PercentageChange = Math.Round(percentageChange, 2),
                IsPositive = isPositive
            };
        }
    }
}
