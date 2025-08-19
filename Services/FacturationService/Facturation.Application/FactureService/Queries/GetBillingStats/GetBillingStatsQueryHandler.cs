using AutoMapper;
using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetBillingStats
{
    public class GetBillingStatsQueryHandler : IRequestHandler<GetBillingStatsQuery, BillingStatsDto>
    {
        private readonly IFactureRepository _factureRepository;
        public GetBillingStatsQueryHandler(IFactureRepository factureRepository, IMapper mapper)
        {
            _factureRepository = factureRepository;
        }
        public async Task<BillingStatsDto> Handle(GetBillingStatsQuery request, CancellationToken cancellationToken)
        {
            var stats = await _factureRepository.GetBillingStatsAsync(request.cliniqueId);
            return new BillingStatsDto
            {
                Revenue = stats.Revenue,
                PendingAmount = stats.PendingAmount,
                OverdueAmount = stats.OverdueAmount,
                PaymentRate = stats.PaymentRate,
            };
        }
    }
}