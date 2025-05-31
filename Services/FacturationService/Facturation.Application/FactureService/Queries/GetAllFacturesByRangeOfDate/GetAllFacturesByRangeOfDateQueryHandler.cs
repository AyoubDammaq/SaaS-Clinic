using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetAllFacturesByRangeOfDate
{
    public class GetAllFacturesByRangeOfDateQueryHandler : IRequestHandler<GetAllFacturesByRangeOfDateQuery, IEnumerable<GetFacturesResponse>>
    {
        private readonly IFactureRepository _factureRepository;
        public GetAllFacturesByRangeOfDateQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }
        public async Task<IEnumerable<GetFacturesResponse>> Handle(GetAllFacturesByRangeOfDateQuery request, CancellationToken cancellationToken)
        {
            if (request.startDate > request.endDate)
                throw new ArgumentException("La date de début ne peut pas être postérieure à la date de fin.");

            var factures = await _factureRepository.GetAllFacturesByRangeOfDateAsync(request.startDate, request.endDate);

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
