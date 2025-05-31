using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetAllFacturesByState
{
    public class GetAllFacturesByStateQueryHandler : IRequestHandler<GetAllFacturesByStateQuery, IEnumerable<GetFacturesResponse>>
    {
        private readonly IFactureRepository _factureRepository;
        public GetAllFacturesByStateQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }
        public async Task<IEnumerable<GetFacturesResponse>> Handle(GetAllFacturesByStateQuery request, CancellationToken cancellationToken)
        {
            var factures = await _factureRepository.GetAllFacturesByStateAsync(request.status);

            return factures.Select(facture => new GetFacturesResponse
            {
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                DateEmission = facture.DateEmission,
                MontantTotal = facture.MontantTotal,
                Status = facture.Status
            });
        }
    }
}
