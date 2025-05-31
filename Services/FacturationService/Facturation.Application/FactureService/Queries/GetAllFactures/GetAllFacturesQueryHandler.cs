using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetAllFactures
{
    public class GetAllFacturesQueryHandler : IRequestHandler<GetAllFacturesQuery, IEnumerable<GetFacturesResponse>>
    {
        private readonly IFactureRepository _factureRepository;
        public GetAllFacturesQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }
        public async Task<IEnumerable<GetFacturesResponse>> Handle(GetAllFacturesQuery request, CancellationToken cancellationToken)
        {
            var factures = await _factureRepository.GetAllFacturesAsync();

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
