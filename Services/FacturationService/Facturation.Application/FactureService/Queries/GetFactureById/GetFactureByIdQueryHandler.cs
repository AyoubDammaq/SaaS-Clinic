using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetFactureById
{
    public class GetFactureByIdQueryHandler : IRequestHandler<GetFactureByIdQuery, GetFacturesResponse>
    {
        private readonly IFactureRepository _factureRepository;
        public GetFactureByIdQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }
        public async Task<GetFacturesResponse> Handle(GetFactureByIdQuery request, CancellationToken cancellationToken)
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
