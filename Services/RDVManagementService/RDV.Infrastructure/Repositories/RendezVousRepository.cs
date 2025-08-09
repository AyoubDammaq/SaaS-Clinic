using Microsoft.EntityFrameworkCore;
using RDV.Domain.Entities;
using RDV.Domain.Enums;
using RDV.Domain.Interfaces;
using RDV.Infrastructure.Data;
using System.Net.Http;
using System.Text.Json;

namespace RDV.Infrastructure.Repositories
{
    public class RendezVousRepository : IRendezVousRepository
    {
        private readonly RendezVousDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public RendezVousRepository(RendezVousDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<RendezVous>> GetAllRendezVousAsync()
        {
            return await _context.RendezVous.ToListAsync();
        }

        public async Task<RendezVous> GetRendezVousByIdAsync(Guid id)
        {
            return await _context.RendezVous.FindAsync(id);
        }

        public async Task CreateRendezVousAsync(RendezVous rendezVous)
        {
            await _context.RendezVous.AddAsync(rendezVous);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRendezVousAsync(Guid id, RendezVous rendezVous)
        {
            _context.RendezVous.Update(rendezVous);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnnulerRendezVousAsync(Guid id)
        {
            var rendezVous = await _context.RendezVous.FindAsync(id);
            if (rendezVous == null)
            {
                throw new Exception("Rendez-vous non trouvé");
            }
            rendezVous.Statut = RDVstatus.ANNULE;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByPatientIdAsync(Guid patientId)
        {
            return await _context.RendezVous.Where(r => r.PatientId == patientId).ToListAsync();
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByMedecinIdAsync(Guid medecinId)
        {
            return await _context.RendezVous.Where(r => r.MedecinId == medecinId).ToListAsync();
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByDateAsync(DateTime date)
        {
            return await _context.RendezVous.Where(r => r.DateHeure.Date == date.Date).ToListAsync();
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByStatutAsync(RDVstatus statut)
        {
            return await _context.RendezVous.Where(r => r.Statut == statut).ToListAsync();
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByPeriod(DateTime start, DateTime end)
        {
            return await _context.RendezVous
                .Where(r => r.DateHeure >= start && r.DateHeure <= end)
                .ToListAsync();
        }

        public async Task<IEnumerable<RendezVous>> GetRendezVousByMedecinAndDateAsync(Guid medecinId, DateTime date)
        {
            return await _context.RendezVous
                .Where(r => r.MedecinId == medecinId && r.DateHeure.Date == date.Date && r.Statut == RDVstatus.CONFIRME)
                .ToListAsync();
        }

        public async Task<int> CountPendingRDVByDoctorAsync(Guid medecinId)
        {
            return await _context.RendezVous
                .CountAsync(r => r.MedecinId == medecinId && r.Statut == RDVstatus.EN_ATTENTE);
        }

        public async Task<int> CountPendingRDVByClinicAsync(Guid clinicId)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"http://doctorService:8085/api/Medecin/medecinsIds/clinique/{clinicId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erreur lors de l'appel à l'API Doctor pour la clinique {clinicId}");
            }


            var json = await response.Content.ReadAsStringAsync();
            var medecinIds = JsonSerializer.Deserialize<List<Guid>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (medecinIds == null || !medecinIds.Any())
            {
                return 0;
            }

            return await _context.RendezVous
                .Where(r => medecinIds.Contains(r.MedecinId) && r.Statut == RDVstatus.EN_ATTENTE)
                .CountAsync();
        }

        public async Task<int> CountByMedecinIdsAsync(List<Guid> medecinIds)
        {
            return await _context.RendezVous
                .CountAsync(r => medecinIds.Contains(r.MedecinId));
        }

        public async Task<int> CountDistinctPatientsByMedecinIdsAsync(List<Guid> medecinIds)
        {
            return await _context.RendezVous
                .Where(r => medecinIds.Contains(r.MedecinId))
                .Select(r => r.PatientId)
                .Distinct()
                .CountAsync();
        }

        public async Task ConfirmerRendezVousParMedecin(Guid rendezVousId)
        {
            var rendezVous = await _context.RendezVous.FindAsync(rendezVousId);
            if (rendezVous == null)
            {
                throw new Exception("Rendez-vous non trouvé");
            }
            rendezVous.Statut = RDVstatus.CONFIRME;
            await _context.SaveChangesAsync();
        }

        public async Task AnnulerRendezVousParMedecin(Guid rendezVousId, string justification)
        {
            var rendezVous = await _context.RendezVous.FindAsync(rendezVousId);
            if (rendezVous == null)
            {
                throw new Exception("Rendez-vous non trouvé");
            }
            rendezVous.Statut = RDVstatus.ANNULE;
            rendezVous.Commentaire = justification;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteRendezVousPourMedecinEtDate(Guid medecinId, DateTime dateHeure)
        {
            return await _context.RendezVous
                .AnyAsync(r => r.MedecinId == medecinId && r.DateHeure == dateHeure);
        }

        public async Task<int> CountByMedecinsIdsAndDateAsync(List<Guid> medecinsIds, DateTime date)
        {
            var dateOnly = date.Date;

            return await _context.RendezVous
                .Where(r => medecinsIds.Contains(r.MedecinId) &&
                            r.DateHeure.Date == dateOnly)
                .CountAsync();
        }
    }
}
