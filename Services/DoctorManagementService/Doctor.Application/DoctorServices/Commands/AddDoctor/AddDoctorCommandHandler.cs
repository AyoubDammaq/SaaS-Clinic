using Doctor.Application.DTOs;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.AddDoctor
{
    public class AddDoctorCommandHandler : IRequestHandler<AddDoctorCommand, GetMedecinRequestDto>
    {
        private readonly IMedecinRepository _medecinRepository;

        public AddDoctorCommandHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }

        public async Task<GetMedecinRequestDto> Handle(AddDoctorCommand request, CancellationToken cancellationToken)
        {
            if (request.createMedecinDto == null)
            {
                throw new ArgumentNullException(nameof(request.createMedecinDto), "Le médecin ne peut pas être nul.");
            }

            var dto = request.createMedecinDto;

            // Construction de l'objet domaine
            var medecin = new Medecin
            {
                Id = Guid.NewGuid(),
                Prenom = dto.Prenom,
                Nom = dto.Nom,
                Specialite = dto.Specialite,
                Email = dto.Email,
                Telephone = dto.Telephone,
                PhotoUrl = dto.PhotoUrl,
                Disponibilites = dto.Disponibilites?.Select(d => new Disponibilite
                {
                    Jour = d.Jour,
                    HeureDebut = d.HeureDebut,
                    HeureFin = d.HeureFin
                }).ToList() ?? new List<Disponibilite>()
            };


            medecin.AddDoctorEvent();

            await _medecinRepository.AddAsync(medecin);

            return new GetMedecinRequestDto
            {
                Id = medecin.Id,
                Prenom = medecin.Prenom,
                Nom = medecin.Nom,
                Specialite = medecin.Specialite,
                Email = medecin.Email,
                Telephone = medecin.Telephone,
                PhotoUrl = medecin.PhotoUrl,
                Disponibilites = medecin.Disponibilites.ToList()
            };
        }
    }
}
