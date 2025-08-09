using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetRevenusMensuels
{
    public class GetRevenusMensuelsQueryHandler : IRequestHandler<GetRevenusMensuelsQuery, decimal>
    {
        private readonly IFactureRepository _factureRepository;
        public GetRevenusMensuelsQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository ?? throw new ArgumentNullException(nameof(factureRepository));
        }
        public async Task<decimal> Handle(GetRevenusMensuelsQuery request, CancellationToken cancellationToken)
        {
            if (request.clinicId == Guid.Empty)
            {
                throw new ArgumentException("Clinic ID cannot be empty.", nameof(request.clinicId));
            }
            return await _factureRepository.GetRevenusMensuelsAsync(request.clinicId);
        }
    }
}
