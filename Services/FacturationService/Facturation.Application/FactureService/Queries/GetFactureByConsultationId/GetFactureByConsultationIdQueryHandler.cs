using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetFactureByConsultationId
{
    public class GetFactureByConsultationIdQueryHandler : IRequestHandler<GetFactureByConsultationIdQuery, FactureDto>
    {
        private readonly IFactureRepository _factureRepository;
        public GetFactureByConsultationIdQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }

        public async Task<FactureDto> Handle(GetFactureByConsultationIdQuery request, CancellationToken cancellationToken)
        {
            if (request.consultationId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.", nameof(request.consultationId));

            var facture = await _factureRepository.GetFactureByConsultationIdAsync(request.consultationId);

            if (facture == null)
                throw new InvalidOperationException($"Aucune facture trouvée pour la consultation {request.consultationId}");

            return new FactureDto
            {
                Id = facture.Id,
                PatientId = facture.PatientId,
                ConsultationId = facture.ConsultationId,
                ClinicId = facture.ClinicId,
                DateEmission = facture.DateEmission,
                MontantPaye = facture.MontantPaye,
                MontantTotal = facture.MontantTotal,
                Status = facture.Status
            };
        }
    }
}
