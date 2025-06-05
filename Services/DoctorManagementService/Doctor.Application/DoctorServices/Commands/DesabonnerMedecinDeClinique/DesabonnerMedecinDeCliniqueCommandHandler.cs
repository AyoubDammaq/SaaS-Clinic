using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.DoctorServices.Commands.DesabonnerMedecinDeClinique
{
    public class DesabonnerMedecinDeCliniqueCommandHandler : IRequestHandler<DesabonnerMedecinDeCliniqueCommand>
    {
        private readonly IMedecinRepository _medecinRepository;
        public DesabonnerMedecinDeCliniqueCommandHandler(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }
        public async Task Handle(DesabonnerMedecinDeCliniqueCommand request, CancellationToken cancellationToken)
        {
            if (request.medecinId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(request.medecinId));
            }

            var medecin = await _medecinRepository.GetByIdAsync(request.medecinId);
            if (medecin.CliniqueId is Guid cliniqueId)
            {
                medecin.DesabonnerDeCliniqueEvent(cliniqueId);
            }
            else
            {
                throw new InvalidOperationException("Le médecin n'est abonné à aucune clinique.");
            }

            await _medecinRepository.DesabonnerMedecinDeCliniqueAsync(request.medecinId);
        }
    }
}
