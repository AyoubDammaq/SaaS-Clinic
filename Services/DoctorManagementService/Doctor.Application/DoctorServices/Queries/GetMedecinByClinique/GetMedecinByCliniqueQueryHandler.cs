using Doctor.Application.DTOs;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetMedecinByClinique
{
    public class GetMedecinByCliniqueQueryHandler : IRequestHandler<GetMedecinByCliniqueQuery, IEnumerable<GetMedecinRequestDto>>
    {
        private readonly IMedecinRepository _medecinRepository;
        public GetMedecinByCliniqueQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<IEnumerable<GetMedecinRequestDto>> Handle(GetMedecinByCliniqueQuery request, CancellationToken cancellationToken)
        {
            if (request.cliniqueId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la clinique est invalide.", nameof(request.cliniqueId));
            }

            var medecins = await _medecinRepository.GetMedecinByCliniqueIdAsync(request.cliniqueId);

            return medecins.Select(m => new GetMedecinRequestDto
            {
                Id = m.Id,
                Prenom = m.Prenom,
                Nom = m.Nom,
                Specialite = m.Specialite,
                CliniqueId = m.CliniqueId,
                Email = m.Email,
                Telephone = m.Telephone,
                PhotoUrl = m.PhotoUrl,
                Disponibilites = (List<Disponibilite>)m.Disponibilites
            });
        }
    }
}
