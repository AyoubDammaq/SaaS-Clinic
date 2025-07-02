using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.UpdateDoctor
{
    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand>
    {
        private readonly IMedecinRepository _medecinRepository;
        public UpdateDoctorCommandHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin est invalide.", nameof(request.id));
            }

            if (request.medecinDto == null)
            {
                throw new ArgumentNullException(nameof(request.medecinDto), "Les données du médecin ne peuvent pas être nulles.");
            }

            var medecin = await _medecinRepository.GetByIdAsync(request.id);
            if (medecin == null)
            {
                throw new KeyNotFoundException("Médecin introuvable.");
            }

            medecin.Prenom = request.medecinDto.Prenom;
            medecin.Nom = request.medecinDto.Nom;
            medecin.Specialite = request.medecinDto.Specialite;
            //medecin.CliniqueId = request.medecinDto.CliniqueId ?? Guid.Empty;
            medecin.Email = request.medecinDto.Email;
            medecin.Telephone = request.medecinDto.Telephone;
            medecin.PhotoUrl = request.medecinDto.PhotoUrl;
            medecin.Disponibilites = request.medecinDto.Disponibilites;

            medecin.UpdateDoctorEvent();

            await _medecinRepository.UpdateAsync(medecin);
        }
    }
}
