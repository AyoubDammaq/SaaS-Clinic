using Doctor.Application.DTOs;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Queries.GetDoctorById
{
    public class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, GetMedecinRequestDto>
    {
        private readonly IMedecinRepository _medecinRepository;
        public GetDoctorByIdQueryHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task<GetMedecinRequestDto> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin est invalide.", nameof(request.id));
            }

            var medecin = await _medecinRepository.GetByIdAsync(request.id);
 
            return new GetMedecinRequestDto
            {
                Id = medecin.Id,
                Prenom = medecin.Prenom,
                Nom = medecin.Nom,
                Specialite = medecin.Specialite,
                CliniqueId = medecin.CliniqueId,
                Email = medecin.Email,
                Telephone = medecin.Telephone,
                PhotoUrl = medecin.PhotoUrl,
                Disponibilites = (List<Disponibilite>)medecin.Disponibilites
            };
        }
    }
}
