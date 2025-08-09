using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.CountConsultationByDate
{
    public class CountConsultationByDateQueryHandler : IRequestHandler<CountConsultationByDateQuery, int>
    {
        private readonly IConsultationRepository _consultationRepository;

        public CountConsultationByDateQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository ?? throw new ArgumentNullException(nameof(consultationRepository));
        }

        public async Task<int> Handle(CountConsultationByDateQuery request, CancellationToken cancellationToken)
        {
            return await _consultationRepository.CountConsultationByDateAsync(
                request.CliniqueId,
                request.MedecinId,
                request.PatientId,
                request.DateDebut,
                request.DateFin);
        }
    }
}
