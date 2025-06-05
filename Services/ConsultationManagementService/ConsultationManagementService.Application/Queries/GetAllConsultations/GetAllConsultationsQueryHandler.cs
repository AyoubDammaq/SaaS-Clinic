using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.GetAllConsultations
{
    public class GetAllConsultationsQueryHandler : IRequestHandler<GetAllConsultationsQuery, IEnumerable<Consultation>>
    {
        private readonly IConsultationRepository _consultationRepository;
        public GetAllConsultationsQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task<IEnumerable<Consultation>> Handle(GetAllConsultationsQuery request, CancellationToken cancellationToken)
        {
            return await _consultationRepository.GetAllConsultationsAsync();
        }
    }
}
