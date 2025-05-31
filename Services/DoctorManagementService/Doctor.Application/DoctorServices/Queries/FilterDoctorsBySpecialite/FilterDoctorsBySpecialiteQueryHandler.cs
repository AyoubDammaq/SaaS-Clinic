using Doctor.Application.DTOs;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.FilterDoctorsBySpecialite
{
    public class FilterDoctorsBySpecialiteQueryHandler : IRequestHandler<FilterDoctorsBySpecialiteQuery, IEnumerable<GetMedecinRequestDto>>
    {
        private readonly IMedecinRepository _medecinRepository;
        public FilterDoctorsBySpecialiteQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<IEnumerable<GetMedecinRequestDto>> Handle(FilterDoctorsBySpecialiteQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.specialite))
            {
                throw new ArgumentException("La spécialité doit être spécifiée.");
            }
            var medecins = await _medecinRepository.FilterBySpecialiteAsync(request.specialite);

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
