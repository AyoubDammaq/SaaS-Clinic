using ConsultationManagementService.Repositories;
using MediatR;

namespace Consultation.Application.Queries.GetAllConsultations
{
    public class GetAllConsultationsQueryHandler : IRequestHandler<GetAllConsultationsQuery, IEnumerable<ConsultationManagementService.Models.Consultation>>
    {
        private readonly IConsultationRepository _consultationRepository;
        public GetAllConsultationsQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task<IEnumerable<ConsultationManagementService.Models.Consultation>> Handle(GetAllConsultationsQuery request, CancellationToken cancellationToken)
        {
            return await _consultationRepository.GetAllConsultationsAsync();
        }
    }
}
