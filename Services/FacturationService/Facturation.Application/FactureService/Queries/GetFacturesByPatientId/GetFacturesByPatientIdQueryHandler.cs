using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetFacturesByPatientId
{
    public class GetFacturesByPatientIdQueryHandler : IRequestHandler<GetFacturesByPatientIdQuery, IEnumerable<GetFacturesResponse>>
    {
        private readonly IFactureRepository _factureRepository;
        public GetFacturesByPatientIdQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }

        public async Task<IEnumerable<GetFacturesResponse>> Handle(GetFacturesByPatientIdQuery request, CancellationToken cancellationToken)
        {
            if (request.patientId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la facture ne peut pas être vide.", nameof(request.patientId));

            var factures = await _factureRepository.GetFactureByPatientIdAsync(request.patientId);

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
