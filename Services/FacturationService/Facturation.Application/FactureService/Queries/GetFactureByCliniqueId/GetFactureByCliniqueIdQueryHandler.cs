using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetFactureByCliniqueId
{
    public class GetFactureByCliniqueIdQueryHandler : IRequestHandler<GetFactureByCliniqueIdQuery, IEnumerable<GetFacturesResponse>>
    {
        private readonly IFactureRepository _factureRepository;
        public GetFactureByCliniqueIdQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }

        public async Task<IEnumerable<GetFacturesResponse>> Handle(GetFactureByCliniqueIdQuery request, CancellationToken cancellationToken)
        {
            if (request.cliniqueId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la facture ne peut pas être vide.", nameof(request.cliniqueId));

            var factures = await _factureRepository.GetFactureByCliniqueIdAsync(request.cliniqueId);

            return factures.Select(facture => new GetFacturesResponse
            {
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                DateEmission = facture.DateEmission,
                MontantTotal = facture.MontantTotal,
                MontantPaye = facture.MontantPaye,
                Status = facture.Status
            });
        }
    }
}
