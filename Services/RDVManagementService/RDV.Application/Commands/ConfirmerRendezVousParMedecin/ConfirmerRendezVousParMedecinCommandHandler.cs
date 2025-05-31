using MediatR;
using RDV.Domain.Interfaces;

namespace RDV.Application.Commands.ConfirmerRendezVousParMedecin
{
    public class ConfirmerRendezVousParMedecinCommandHandler : IRequestHandler<ConfirmerRendezVousParMedecinCommand>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public ConfirmerRendezVousParMedecinCommandHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task Handle(ConfirmerRendezVousParMedecinCommand request, CancellationToken cancellationToken)
        {
            if (request.rendezVousId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du rendez-vous ne peut pas être vide.", nameof(request.rendezVousId));
            }
            await _rendezVousRepository.ConfirmerRendezVousParMedecin(request.rendezVousId);
        }
    }
}
