using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.GetConsultationsByDoctorId
{
    public class GetConsultationsByDoctorIdQueryHandler : IRequestHandler<GetConsultationsByDoctorIdQuery, IEnumerable<Consultation>>
    {
        private readonly IConsultationRepository _consultationRepository;
        public GetConsultationsByDoctorIdQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task<IEnumerable<Consultation>> Handle(GetConsultationsByDoctorIdQuery request, CancellationToken cancellationToken)
        {
            if (request.doctorId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(request.doctorId));
            }
            return await _consultationRepository.GetConsultationsByDoctorIdAsync(request.doctorId);
        }
    }
}
