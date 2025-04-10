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
            await _disponibiliteRepository.AjouterDisponibiliteAsync(medecinId, nouvelleDispo);
        }

        public async Task SupprimerDisponibilite(Guid disponibiliteId)
        {
            await _disponibiliteRepository.SupprimerDisponibiliteAsync(disponibiliteId);
        }

        public async Task<List<Disponibilite>> GetDisponibilitesByMedecinId(Guid medecinId)
        {
            return await _disponibiliteRepository.ObtenirDisponibilitesParMedecinId(medecinId);
        }

        public async Task<Disponibilite> GetDisponibiliteById(Guid disponibiliteId)
        {
            return await _disponibiliteRepository.ObtenirDisponibiliteParId(disponibiliteId);
        }

        public async Task<List<Disponibilite>> GetDisponibilites()
        {
            return await _disponibiliteRepository.ObtenirDisponibilites();
        }

        public async Task<Disponibilite> GetDisponibiliteWithMedecin(Guid disponibiliteId)
        {
            return await _disponibiliteRepository.ObtenirDisponibiliteAvecMedecin(disponibiliteId);
        }

        public async Task<List<Disponibilite>> ObtenirDisponibilitesParMedecinIdEtDate(Guid medecinId, DateTime date)
        {
            var disponibilites = await _disponibiliteRepository.ObtenirDisponibilitesParMedecinId(medecinId);
            return disponibilites.Where(d => d.Jour == date.DayOfWeek).ToList();
        }
    }

}