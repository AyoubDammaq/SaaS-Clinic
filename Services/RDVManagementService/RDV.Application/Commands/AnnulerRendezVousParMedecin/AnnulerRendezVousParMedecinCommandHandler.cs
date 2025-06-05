using MediatR;
using RDV.Domain.Interfaces;

namespace RDV.Application.Commands.AnnulerRendezVousParMedecin
{
    public class AnnulerRendezVousParMedecinCommandHandler : IRequestHandler<AnnulerRendezVousParMedecinCommand>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public AnnulerRendezVousParMedecinCommandHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task Handle(AnnulerRendezVousParMedecinCommand request, CancellationToken cancellationToken)
        {
            if (request.rendezVousId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du rendez-vous ne peut pas être vide.", nameof(request.rendezVousId));
            }
            if (string.IsNullOrWhiteSpace(request.justification))
            {
                throw new ArgumentException("La justification ne peut pas être vide.", nameof(request.justification));
            }

            var rendezVous = await _rendezVousRepository.GetRendezVousByIdAsync(request.rendezVousId);
            rendezVous.RejeterRendezVousParMedecinEvent(request.justification);

            await _rendezVousRepository.AnnulerRendezVousParMedecin(request.rendezVousId, request.justification);
        }
    }
}
