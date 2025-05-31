using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.AttribuerMedecinAUneClinique
{
    public class AttribuerMedecinAUneCliniqueCommandHandler : IRequestHandler<AttribuerMedecinAUneCliniqueCommand>
    {
        private readonly IMedecinRepository _medecinRepository;
        public AttribuerMedecinAUneCliniqueCommandHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task Handle(AttribuerMedecinAUneCliniqueCommand request, CancellationToken cancellationToken)
        {
            if (request.medecinId == Guid.Empty || request.cliniqueId == Guid.Empty)
            {
                throw new ArgumentException("Les identifiants du médecin et de la clinique ne peuvent pas être vides.");
            }
            await _medecinRepository.AttribuerMedecinAUneCliniqueAsync(request.medecinId, request.cliniqueId);
        }
    }
}
