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
            await _medecinRepository.DesabonnerMedecinDeCliniqueAsync(request.medecinId);
        }
    }
}
