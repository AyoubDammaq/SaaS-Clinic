using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.CountConsultationByPatient
{
    public class CountConsultationByPatientQueryHandler : IRequestHandler<CountConsultationByPatientQuery, int>
    {
        private readonly IConsultationRepository _consultationRepository;
        public CountConsultationByPatientQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository ?? throw new ArgumentNullException(nameof(consultationRepository));
        }
        public async Task<int> Handle(CountConsultationByPatientQuery request, CancellationToken cancellationToken)
        {
            if (request.PatientId == Guid.Empty)
                throw new ArgumentException("Invalid patient ID.", nameof(request.PatientId));
            return await _consultationRepository.CountConsultationByPatient(request.PatientId, request.startDate, request.endDate);
        }
    }
}
