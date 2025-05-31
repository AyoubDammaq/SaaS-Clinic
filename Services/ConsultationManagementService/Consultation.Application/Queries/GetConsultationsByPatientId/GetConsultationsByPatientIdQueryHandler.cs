using ConsultationManagementService.Repositories;
using MediatR;

namespace Consultation.Application.Queries.GetConsultationsByPatientId
{
    public class GetConsultationsByPatientIdQueryHandler : IRequestHandler<GetConsultationsByPatientIdQuery, IEnumerable<ConsultationManagementService.Models.Consultation>>
    {
        private readonly IConsultationRepository _consultationRepository;
        public GetConsultationsByPatientIdQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task<IEnumerable<ConsultationManagementService.Models.Consultation>> Handle(GetConsultationsByPatientIdQuery request, CancellationToken cancellationToken)
        {
            if (request.patientId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du patient ne peut pas être vide.", nameof(request.patientId));
            }
            return await _consultationRepository.GetConsultationsByPatientIdAsync(request.patientId);
        }
    }
}
