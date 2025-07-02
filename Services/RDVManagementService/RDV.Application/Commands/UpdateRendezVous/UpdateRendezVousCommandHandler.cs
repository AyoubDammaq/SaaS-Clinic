using MediatR;
using RDV.Domain.Interfaces;

namespace RDV.Application.Commands.UpdateRendezVous
{
    public class UpdateRendezVousCommandHandler : IRequestHandler<UpdateRendezVousCommand>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public UpdateRendezVousCommandHandler(IRendezVousRepository rendezVousRepository)
        {
            _rendezVousRepository = rendezVousRepository;
        }
        public async Task Handle(UpdateRendezVousCommand request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du rendez-vous ne peut pas être vide.", nameof(request.id));
            }
            if (request.rendezVous == null)
            {
                throw new ArgumentNullException(nameof(request.rendezVous), "Le rendez-vous ne peut pas être nul.");
            }

            // 🔒 Règle métier : empêcher les doubles réservations
            bool dejaPris = await _rendezVousRepository
                .ExisteRendezVousPourMedecinEtDate(request.rendezVous.MedecinId, request.rendezVous.DateHeure);

            if (dejaPris)
            {
                throw new InvalidOperationException("Un rendez-vous existe déjà à cette heure pour ce médecin.");
            }

            request.rendezVous.ModifierRendezVousEvent();   

            await _rendezVousRepository.UpdateRendezVousAsync(request.id, request.rendezVous);
        }
    }
}
