using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.GetNombreConsultations
{
    public class GetNombreConsultationsQueryHandler : IRequestHandler<GetNombreConsultationsQuery, int>
    {
        private readonly IConsultationRepository _consultationRepository;
        public GetNombreConsultationsQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task<int> Handle(GetNombreConsultationsQuery request, CancellationToken cancellationToken)
        {
            return await _consultationRepository.CountConsultationsAsync(request.dateDebut, request.dateFin);
        }
    }
}
