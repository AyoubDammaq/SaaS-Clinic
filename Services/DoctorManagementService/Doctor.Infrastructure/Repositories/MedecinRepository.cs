
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Doctor.Domain.ValueObject;
using Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Net.Http.Json;

namespace Doctor.Infrastructure.Repositories
{
    public class MedecinRepository : IMedecinRepository
    {
        private readonly MedecinDbContext _context;
        private readonly HttpClient _httpClient;

        public MedecinRepository(MedecinDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }
        public async Task<Medecin> GetByIdAsync(Guid id)
        {
            return await _context.Medecins
                .Include(m => m.Disponibilites)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<List<Medecin>> GetAllAsync()
        {
            return await _context.Medecins
                .Include(m => m.Disponibilites)
                .ToListAsync();
        }
        public async Task AddAsync(Medecin medecin)
        {
            await _context.Medecins.AddAsync(medecin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Medecin medecin)
        {
            _context.Medecins.Update(medecin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var medecin = await GetByIdAsync(id);
            if (medecin != null)
            {
                _context.Medecins.Remove(medecin);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Medecin>> FilterBySpecialiteAsync(string specialite)
        {
            return await _context.Medecins
                .Include(m => m.Disponibilites)
                .Where(m => (string.IsNullOrEmpty(specialite) || m.Specialite == specialite))
                .ToListAsync();
        }

        public async Task<List<Medecin>> FilterByNameOrPrenomAsync(string name, string prenom)
        {
            name = name?.Trim().ToLower();
            prenom = prenom?.Trim().ToLower();

            return await _context.Medecins
                .Where(m =>
                    (string.IsNullOrEmpty(name) || m.Nom.ToLower().Contains(name)) &&
                    (string.IsNullOrEmpty(prenom) || m.Prenom.ToLower().Contains(prenom))
                )
                .Include(m => m.Disponibilites)
                .ToListAsync();
        }

        public async Task<List<Medecin>> GetMedecinByCliniqueIdAsync(Guid cliniqueId)
        {
            return await _context.Medecins
                .Where(m => m.CliniqueId == cliniqueId)
                .Include(m => m.Disponibilites)
                .ToListAsync();
        }
        public async Task AttribuerMedecinAUneCliniqueAsync(Guid medecinId, Guid cliniqueId)
        {
            var medecin = await GetByIdAsync(medecinId);
            if (medecin != null)
            {
                medecin.CliniqueId = cliniqueId;
                await UpdateAsync(medecin);
            }
        }
        public async Task DesabonnerMedecinDeCliniqueAsync(Guid medecinId)
        {
            var medecin = await GetByIdAsync(medecinId);
            if (medecin != null)
            {
                medecin.CliniqueId = null;
                await UpdateAsync(medecin);
            }
        }


        public async Task<IEnumerable<StatistiqueMedecin>> GetNombreMedecinBySpecialiteAsync()
        {
            return await _context.Medecins
                .GroupBy(m => m.Specialite)
                .Select(g => new StatistiqueMedecin
                {
                    Cle = g.Key,
                    Nombre = g.Count()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<StatistiqueMedecin>> GetNombreMedecinByCliniqueAsync()
        {
            return await _context.Medecins
                .GroupBy(m => m.CliniqueId)
                .Select(g => new StatistiqueMedecin
                {
                    Cle = g.Key.HasValue ? g.Key.ToString() : "Sans clinique",
                    Nombre = g.Count()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<StatistiqueMedecin>> GetNombreMedecinBySpecialiteDansUneCliniqueAsync(Guid cliniqueId)
        {
            return await _context.Medecins
                .Where(m => m.CliniqueId == cliniqueId)
                .GroupBy(m => m.Specialite)
                .Select(g => new StatistiqueMedecin
                {
                    Cle = g.Key,
                    Nombre = g.Count()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Guid>> GetMedecinsIdsByCliniqueId(Guid cliniqueId)
        {
            return await _context.Medecins
                .Where(m => m.CliniqueId == cliniqueId)
                .Select(m => m.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActiviteMedecin>> GetActivitesMedecinAsync(Guid medecinId)
        {
            // Récupérer le médecin par son ID
            var medecin = await GetByIdAsync(medecinId);
            if (medecin == null) return Enumerable.Empty<ActiviteMedecin>();

            // Récupérer les consultations du médecin
            var responseConsultation = await _httpClient.GetAsync($"http://localhost:5015/api/Consultation/Doctor/{medecinId}");

            // Vérifier si la réponse est réussie
            if (!responseConsultation.IsSuccessStatusCode)
            {
                throw new Exception("Erreur lors de la récupération des consultations");
            }

            // Récupérer les rendez-vous du médecin
            var responseRendezVous = await _httpClient.GetAsync($"http://localhost:5133/api/RendezVous/medecin/{medecinId}");

            // Vérifier si la réponse est réussie
            if (!responseRendezVous.IsSuccessStatusCode)
            {
                throw new Exception("Erreur lors de la récupération des rendez-vous");
            }

            // Lire le contenu des réponses
            var consultations = await responseConsultation.Content.ReadFromJsonAsync<IEnumerable<object>>();
            var rendezVous = await responseRendezVous.Content.ReadFromJsonAsync<IEnumerable<object>>();

            // Vérifier si les consultations et les rendez-vous sont nuls
            if (consultations == null && rendezVous == null)
            {
                return Enumerable.Empty<ActiviteMedecin>();
            }

            // Compter le nombre de consultations et de rendez-vous
            var nombreConsultations = consultations?.Count() ?? 0;
            var nombreRendezVous = rendezVous?.Count() ?? 0;

            // Créer une liste d'activités du médecin
            return new List<ActiviteMedecin>
            {
                new ActiviteMedecin
                {
                    MedecinId = medecinId,
                    NomComplet = medecin.Prenom + " " + medecin.Nom,
                    NombreConsultations = nombreConsultations,
                    NombreRendezVous = nombreRendezVous
                }
            };
        }
    }
}
