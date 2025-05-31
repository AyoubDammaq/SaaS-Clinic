using Doctor.Application.DTOs;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetAllDoctors
{
    public class GetAllDoctorsQueryHandler : IRequestHandler<GetAllDoctorsQuery, IEnumerable<GetMedecinRequestDto>>
    {
        private readonly IMedecinRepository _medecinRepository;
        public GetAllDoctorsQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<IEnumerable<GetMedecinRequestDto>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
        {
            var medecins = await _medecinRepository.GetAllAsync();

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
