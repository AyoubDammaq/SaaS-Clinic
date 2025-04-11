using DoctorManagementService.Models;
using DoctorManagementService.Repositories;

namespace DoctorManagementService.Services
{
    public class DisponibiliteService : IDisponibiliteService
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public DisponibiliteService(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }

        public async Task AjouterDisponibilite(Guid medecinId, Disponibilite nouvelleDispo)
        {
            if (nouvelleDispo == null)
                throw new ArgumentNullException(nameof(nouvelleDispo), "La disponibilité ne peut pas être null.");

            if (nouvelleDispo.HeureDebut >= nouvelleDispo.HeureFin)
                throw new ArgumentException("L'heure de début doit être inférieure à l'heure de fin.");

            await _disponibiliteRepository.AjouterDisponibiliteAsync(medecinId, nouvelleDispo);
        }

        public async Task SupprimerDisponibilite(Guid disponibiliteId)
        {
            if (disponibiliteId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la disponibilité ne peut pas être vide.", nameof(disponibiliteId));

            var disponibilite = await _disponibiliteRepository.ObtenirDisponibiliteParId(disponibiliteId);
            if (disponibilite == null)
                throw new KeyNotFoundException("La disponibilité spécifiée n'existe pas.");

            await _disponibiliteRepository.SupprimerDisponibiliteAsync(disponibiliteId);
        }

        public async Task<List<Disponibilite>> GetDisponibilitesByMedecinId(Guid medecinId)
        {
            if (medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(medecinId));

            return await _disponibiliteRepository.ObtenirDisponibilitesParMedecinId(medecinId);
        }

        public async Task<Disponibilite> GetDisponibiliteById(Guid disponibiliteId)
        {
            if (disponibiliteId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la disponibilité ne peut pas être vide.", nameof(disponibiliteId));

            var disponibilite = await _disponibiliteRepository.ObtenirDisponibiliteParId(disponibiliteId);
            if (disponibilite == null)
                throw new KeyNotFoundException("La disponibilité spécifiée n'existe pas.");

            return disponibilite;
        }

        public async Task<List<Disponibilite>> GetDisponibilites()
        {
            var disponibilites = await _disponibiliteRepository.ObtenirDisponibilites();
            if (disponibilites == null || !disponibilites.Any())
                throw new InvalidOperationException("Aucune disponibilité trouvée.");

            return disponibilites;
        }

        public async Task<List<Disponibilite>> ObtenirDisponibilitesParMedecinIdEtDate(Guid medecinId, DateTime date)
        {
            if (medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(medecinId));

            var disponibilites = await _disponibiliteRepository.ObtenirDisponibilitesParMedecinId(medecinId);
            if (disponibilites == null || !disponibilites.Any())
                throw new KeyNotFoundException("Aucune disponibilité trouvée pour le médecin spécifié.");

            return disponibilites.Where(d => d.Jour == date.DayOfWeek).ToList();
        }

        public async Task<Medecin> ObtenirDisponibiliteAvecDate(DateTime date, TimeSpan? heureDebut, TimeSpan? heureFin)
        {
            if (heureDebut.HasValue && heureFin.HasValue && heureDebut >= heureFin)
                throw new ArgumentException("L'heure de début doit être inférieure à l'heure de fin.");

            var disponibilites = await _disponibiliteRepository.ObtenirMedecinsDisponiblesAsync(date, heureDebut, heureFin);
            if (disponibilites == null || !disponibilites.Any())
                throw new KeyNotFoundException("Aucun médecin disponible trouvé pour les critères spécifiés.");

            return disponibilites.FirstOrDefault();
        }
    }

}