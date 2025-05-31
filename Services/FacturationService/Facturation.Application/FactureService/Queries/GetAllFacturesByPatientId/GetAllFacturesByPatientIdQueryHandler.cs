using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetAllFacturesByPatientId
{
    public class GetAllFacturesByPatientIdQueryHandler : IRequestHandler<GetAllFacturesByPatientIdQuery, IEnumerable<GetFacturesResponse>>
    {
        private readonly IFactureRepository _factureRepository;
        public GetAllFacturesByPatientIdQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }
        public async Task<IEnumerable<GetFacturesResponse>> Handle(GetAllFacturesByPatientIdQuery request, CancellationToken cancellationToken)
        {
            if (request.patientId == Guid.Empty)
                throw new ArgumentException("L'identifiant du patient ne peut pas être vide.", nameof(request.patientId));

            var factures = await _factureRepository.GetAllFacturesByPatientIdAsync(request.patientId);

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
