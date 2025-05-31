using Doctor.Application.DTOs;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetNombreMedecinBySpecialiteDansUneClinique
{
    public class GetNombreMedecinBySpecialiteDansUneCliniqueQueryHandler : IRequestHandler<GetNombreMedecinBySpecialiteDansUneCliniqueQuery, IEnumerable<StatistiqueMedecinDTO>>
    {
        private readonly IMedecinRepository _medecinRepository;
        public GetNombreMedecinBySpecialiteDansUneCliniqueQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<IEnumerable<StatistiqueMedecinDTO>> Handle(GetNombreMedecinBySpecialiteDansUneCliniqueQuery request, CancellationToken cancellationToken)
        {
            if (request.cliniqueId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la clinique est invalide.", nameof(request.cliniqueId));
            }
            var statistiques = await _medecinRepository.GetNombreMedecinBySpecialiteDansUneCliniqueAsync(request.cliniqueId);

            return statistiques.Select(s => new StatistiqueMedecinDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            });
        }
    }
}
