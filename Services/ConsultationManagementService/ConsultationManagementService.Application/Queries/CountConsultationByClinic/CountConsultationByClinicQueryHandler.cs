using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.CountConsultationByClinic
{
    public class CountConsultationByClinicQueryHandler : IRequestHandler<CountConsultationByClinicQuery, int>
    {
        private readonly IConsultationRepository _consultationRepository;
        public CountConsultationByClinicQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository ?? throw new ArgumentNullException(nameof(consultationRepository));
        }
        public async Task<int> Handle(CountConsultationByClinicQuery request, CancellationToken cancellationToken)
        {
            if (request.ClinicId == Guid.Empty)
                throw new ArgumentException("Invalid clinic ID.", nameof(request.ClinicId));
            return await _consultationRepository.CountConsultationByClinic(request.ClinicId, request.startDate, request.endDate);
        }
    }
}
