using Facturation.Application.DTOs;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Queries.GetFacturesByFilter
{
    public class GetFacturesByFilterQueryHandler : IRequestHandler<GetFacturesByFilterQuery, IEnumerable<FactureDto>>
    {
        private readonly IFactureRepository _factureRepository;

        public GetFacturesByFilterQueryHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
        }

        public async Task<IEnumerable<FactureDto>> Handle(GetFacturesByFilterQuery request, CancellationToken cancellationToken)
        {
            var factures = await _factureRepository.GetByFilterAsync(request.PatientId, request.ClinicId, request.Status);

            return factures.Select(f => new FactureDto
            {
                Id = f.Id,
                PatientId = f.PatientId,
                ConsultationId = f.ConsultationId,
                ClinicId = f.ClinicId,
                DateEmission = f.DateEmission,
                MontantTotal = f.MontantTotal,
                Status = f.Status
            });
        }
    }
}
