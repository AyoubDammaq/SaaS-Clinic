using Doctor.Application.Interfaces;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;

namespace Doctor.Application.Services
{
    public class DisponibiliteService : IDisponibiliteService
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public DisponibiliteService(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }

        public async Task AjouterDisponibilite(Disponibilite nouvelleDispo)
        {
            if (nouvelleDispo == null)
                throw new ArgumentNullException(nameof(nouvelleDispo), "La disponibilité ne peut pas être null.");

            if (nouvelleDispo.HeureDebut >= nouvelleDispo.HeureFin)
                throw new ArgumentException("L'heure de début doit être inférieure à l'heure de fin.");

            await _disponibiliteRepository.AjouterDisponibiliteAsync(nouvelleDispo);
        }

        public async Task UpdateDisponibilite(Disponibilite disponibilite)
        {
            if (disponibilite == null)
                throw new ArgumentNullException(nameof(disponibilite), "La disponibilité ne peut pas être null.");
            if (disponibilite.HeureDebut >= disponibilite.HeureFin)
                throw new ArgumentException("L'heure de début doit être inférieure à l'heure de fin.");
            await _disponibiliteRepository.UpdateDisponibiliteAsync(disponibilite);
        }

        public async Task SupprimerDisponibilite(Guid disponibiliteId)
        {
            if (disponibiliteId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la disponibilité ne peut pas être vide.", nameof(disponibiliteId));

            var disponibilite = await _disponibiliteRepository.ObtenirDisponibiliteParIdAsync(disponibiliteId);
            if (disponibilite == null)
                throw new KeyNotFoundException("La disponibilité spécifiée n'existe pas.");

            await _disponibiliteRepository.SupprimerDisponibiliteAsync(disponibiliteId);
        }

        public async Task<Disponibilite> GetDisponibiliteById(Guid disponibiliteId)
        {
            if (disponibiliteId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la disponibilité ne peut pas être vide.", nameof(disponibiliteId));

            var disponibilite = await _disponibiliteRepository.ObtenirDisponibiliteParIdAsync(disponibiliteId);
            //if (disponibilite == null)
            //  throw new KeyNotFoundException("La disponibilité spécifiée n'existe pas.");

            return disponibilite ?? new Disponibilite();
        }

        public async Task<List<Disponibilite>> GetDisponibilites()
        {
            var disponibilites = await _disponibiliteRepository.ObtenirToutesDisponibilitesAsync();
            //if (disponibilites == null || !disponibilites.Any())
            //  throw new InvalidOperationException("Aucune disponibilité trouvée.");

            return disponibilites ?? new List<Disponibilite>();
        }

        public async Task<List<Disponibilite>> GetDisponibilitesByMedecinId(Guid medecinId)
        {
            if (medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(medecinId));

            var disponibilites = await _disponibiliteRepository.ObtenirDisponibilitesParMedecinIdAsync(medecinId);

            return disponibilites ?? new List<Disponibilite>();
        }

        public async Task<List<Disponibilite>> GetDisponibilitesByMedecinIdAndJour(Guid medecinId, DayOfWeek jour)
        {
            if (medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(medecinId));
            var disponibilites =  await _disponibiliteRepository.ObtenirDisponibilitesParJourAsync(medecinId, jour);

            return disponibilites ?? new List<Disponibilite>();
        }

        public async Task<List<Medecin>> GetMedecinsDisponibles(DateTime date, TimeSpan? heureDebut, TimeSpan? heureFin)
        {
            if (date == default)
                throw new ArgumentException("La date ne peut pas être vide.", nameof(date));
            var medecins = await _disponibiliteRepository.ObtenirMedecinsDisponiblesAsync(date, heureDebut, heureFin);

            return medecins ?? new List<Medecin>();
        }

        public async Task<bool> IsAvailable(Guid medecinId, DateTime dateTime)
        {
            if (medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(medecinId));
            return await _disponibiliteRepository.EstDisponibleAsync(medecinId, dateTime);
        }

        public async Task<bool> CheckOverlap(Disponibilite dispo)
        {
            if (dispo == null)
                throw new ArgumentNullException(nameof(dispo), "La disponibilité ne peut pas être null.");
            return await _disponibiliteRepository.VerifieChevauchementAsync(dispo);
        }

        public async Task<TimeSpan> GetTotalAvailableTime(Guid medecinId, DateTime dateDebut, DateTime dateFin)
        {
            if (medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(medecinId));
            if (dateDebut >= dateFin)
                throw new ArgumentException("La date de début doit être inférieure à la date de fin.");
            return await _disponibiliteRepository.ObtenirTempsTotalDisponibleAsync(medecinId, dateDebut, dateFin);
        }

        public async Task SupprimerDisponibilitesParMedecinId(Guid medecinId)
        {
            if (medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(medecinId));
            await _disponibiliteRepository.SupprimerDisponibilitesParMedecinIdAsync(medecinId);
        }

        public async Task<List<Disponibilite>> ObtenirDisponibilitesDansIntervalle(Guid medecinId, DateTime start, DateTime end)
        {
            if (medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(medecinId));
            if (start >= end)
                throw new ArgumentException("La date de début doit être inférieure à la date de fin.");
            var disponibilites = await _disponibiliteRepository.ObtenirDisponibilitesDansIntervalleAsync(medecinId, start, end);

            return disponibilites ?? new List<Disponibilite>();
        }
    }
}
