using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.CountConsultationByDoctor
{
    public class CountConsultationByDoctorQueryHandler : IRequestHandler<CountConsultationByDoctorQuery, int>
    {
        private readonly IConsultationRepository _consultationRepository;
        public CountConsultationByDoctorQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository ?? throw new ArgumentNullException(nameof(consultationRepository));
        }
        public async Task<int> Handle(CountConsultationByDoctorQuery request, CancellationToken cancellationToken)
        {
            if (request.MedecinId == Guid.Empty)
                throw new ArgumentException("Invalid doctor ID.", nameof(request.MedecinId));
            return await _consultationRepository.CountConsultationByDoctor(request.MedecinId, request.startDate, request.endDate);
        }
    }
}
