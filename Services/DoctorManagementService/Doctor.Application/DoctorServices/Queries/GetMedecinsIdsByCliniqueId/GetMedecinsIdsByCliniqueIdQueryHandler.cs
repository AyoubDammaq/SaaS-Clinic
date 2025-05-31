using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetMedecinsIdsByCliniqueId
{
    public class GetMedecinsIdsByCliniqueIdQueryHandler : IRequestHandler<GetMedecinsIdsByCliniqueIdQuery, IEnumerable<Guid>>
    {
        private readonly IMedecinRepository _medecinRepository;
        public GetMedecinsIdsByCliniqueIdQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<IEnumerable<Guid>> Handle(GetMedecinsIdsByCliniqueIdQuery request, CancellationToken cancellationToken)
        {
            if (request.cliniqueId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la clinique est invalide.", nameof(request.cliniqueId));
            }
            var medecinsIds = await _medecinRepository.GetMedecinsIdsByCliniqueId(request.cliniqueId);

            return medecinsIds;
        }
    }
}
