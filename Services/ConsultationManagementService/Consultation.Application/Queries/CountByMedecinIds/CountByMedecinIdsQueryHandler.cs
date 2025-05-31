using ConsultationManagementService.Repositories;
using MediatR;

namespace Consultation.Application.Queries.CountByMedecinIds
{
    public class CountByMedecinIdsQueryHandler : IRequestHandler<CountByMedecinIdsQuery, int>
    {
        private readonly IConsultationRepository _consultationRepository;
        public CountByMedecinIdsQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task<int> Handle(CountByMedecinIdsQuery request, CancellationToken cancellationToken)
        {
            if (request.medecinIds == null || !request.medecinIds.Any())
            {
                throw new ArgumentException("La liste des identifiants de médecins ne peut pas être vide.", nameof(request.medecinIds));
            }
            return await _consultationRepository.CountByMedecinIdsAsync(request.medecinIds);
        }
    }
}
