using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.CountNouveauxPatientsByDoctor
{
    public class CountNouveauxPatientsByDoctorQueryHandler : IRequestHandler<CountNouveauxPatientsByDoctorQuery, int>
    {
        private readonly IConsultationRepository _consultationRepository;
        public CountNouveauxPatientsByDoctorQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository ?? throw new ArgumentNullException(nameof(consultationRepository));
        }
        public async Task<int> Handle(CountNouveauxPatientsByDoctorQuery request, CancellationToken cancellationToken)
        {
            return await _consultationRepository.CountNouveauxPatientsByDoctorAsync(request.medecinId, request.startDate, request.endDate);
        }
    }
}
