using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.GetConsultationById
{
    public class GetConsultationByIdQueryHandler : IRequestHandler<GetConsultationByIdQuery, Consultation?>
    {
        private readonly IConsultationRepository _consultationRepository;
        public GetConsultationByIdQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task<Consultation?> Handle(GetConsultationByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.", nameof(request.id));
            }
            return await _consultationRepository.GetConsultationByIdAsync(request.id);
        }
    }
}
