using ConsultationManagementService.Application.Queries.CountNouveauxPatientsByDoctor;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.CountNouveauxPatientsByClinic
{
    public class CountNouveauxPatientsByClinicQueryHandler : IRequestHandler<CountNouveauxPatientsByClinicQuery, int>
    {
        private readonly IConsultationRepository _consultationRepository;
        public CountNouveauxPatientsByClinicQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository ?? throw new ArgumentNullException(nameof(consultationRepository));
        }
        public async Task<int> Handle(CountNouveauxPatientsByClinicQuery request, CancellationToken cancellationToken)
        {
            return await _consultationRepository.CountNouveauxPatientsByClinicAsync(request.ClinicId, request.startDate, request.endDate);
        }
    }
}
