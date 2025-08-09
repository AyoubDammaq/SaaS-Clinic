using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.PaiementService.Queries.GetDernierPaiementByPatientId
{
    public class GetDernierPaiementByPatientIdQueryHandler : IRequestHandler<GetDernierPaiementByPatientIdQuery, RecentPaiementDto?>
    {
        private readonly IPaiementRepository _paiementRepository;
        public GetDernierPaiementByPatientIdQueryHandler(IPaiementRepository paiementRepository)
        {
            _paiementRepository = paiementRepository ?? throw new ArgumentNullException(nameof(paiementRepository));
        }
        public async Task<RecentPaiementDto?> Handle(GetDernierPaiementByPatientIdQuery request, CancellationToken cancellationToken)
        {
            if (request.PatientId == Guid.Empty)
                throw new ArgumentException("L'identifiant du patient ne peut pas être vide.", nameof(request.PatientId));
            // Récupérer le dernier paiement du patient
            var recentPayment = await _paiementRepository.GetDernierPaiementByPatientIdAsync(request.PatientId);

            return recentPayment != null ? new RecentPaiementDto
            {
                Montant = recentPayment.Montant,
                DatePaiement = recentPayment.DatePaiement,
            } : null;
        }
    }
}
