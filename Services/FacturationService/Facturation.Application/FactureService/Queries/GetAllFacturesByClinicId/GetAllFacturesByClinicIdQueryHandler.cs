using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Facturation.Application.FactureService.Queries.GetAllFacturesByClinicId
{
    public class GetAllFacturesByClinicIdQueryHandler : IRequestHandler<GetAllFacturesByClinicIdQuery, GetFacturesResponse>
    {
        private readonly IFactureRepository _factureRepository;
        public GetAllFacturesByClinicIdQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }
        public async Task<GetFacturesResponse> Handle(GetAllFacturesByClinicIdQuery request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la facture ne peut pas être vide.", nameof(request.id));

            var facture = await _factureRepository.GetFactureByIdAsync(request.id);

            return new GetFacturesResponse
            {
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                DateEmission = facture.DateEmission,
                MontantTotal = facture.MontantTotal,
                Status = facture.Status
            };
        }
    }
}
