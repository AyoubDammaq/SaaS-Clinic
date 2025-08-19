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

            // Récupérer l'entité existante depuis la base
            var existingRdv = await _rendezVousRepository.GetRendezVousByIdAsync(request.id);
            if (existingRdv == null)
                throw new KeyNotFoundException("Le rendez-vous à mettre à jour n'existe pas.");

            // 🔒 Règle métier : empêcher les doubles réservations
            bool dejaPris = await _rendezVousRepository
                .ExisteRendezVousPourMedecinEtDate(request.rendezVous.MedecinId, request.rendezVous.DateHeure, request.id); ;

            if (dejaPris)
            {
                throw new InvalidOperationException("Un rendez-vous existe déjà à cette heure pour ce médecin.");
            }

            // 🔹 Appliquer uniquement les champs modifiables
            existingRdv.DateHeure = request.rendezVous.DateHeure;
            existingRdv.Commentaire = request.rendezVous.Commentaire;

            // 🔹 Ne pas modifier le statut
            // existingRdv.Statut reste inchangé

            // Déclencher l'événement de modification
            existingRdv.ModifierRendezVousEvent();

            // Sauvegarder les modifications
            await _rendezVousRepository.UpdateRendezVousAsync(existingRdv.Id, existingRdv);
        }
    }
}
