using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetNombreTotalMedecins
{
    public class GetNombreTotalMedecinsQueryHandler : IRequestHandler<GetNombreTotalMedecinsQuery, int>
    {
        private readonly IMedecinRepository _medecinRepository;
        public GetNombreTotalMedecinsQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<int> Handle(GetNombreTotalMedecinsQuery request, CancellationToken cancellationToken)
        {
            return await _medecinRepository.GetNombreTotalMedecinsAsync();
        }
    }
}
