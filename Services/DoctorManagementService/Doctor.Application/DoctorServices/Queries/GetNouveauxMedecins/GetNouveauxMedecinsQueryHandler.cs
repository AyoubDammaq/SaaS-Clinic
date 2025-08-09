using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetNouveauxMedecins
{
    public class GetNouveauxMedecinsQueryHandler : IRequestHandler<GetNouveauxMedecinsQuery, int>
    {
        private readonly IMedecinRepository _medecinRepository;
        public GetNouveauxMedecinsQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<int> Handle(GetNouveauxMedecinsQuery request, CancellationToken cancellationToken)
        {
            return await _medecinRepository.GetNouveauxMedecinsAsync(request.StartDate, request.EndDate);
        }
    }
}
