using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using Clinic.Domain.ValueObject;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Http.Json;


namespace Clinic.Infrastructure.Repositories
{
    public class CliniqueRepository : ICliniqueRepository
    {
        private readonly CliniqueDbContext _context;
        private readonly HttpClient _httpClient;

        public CliniqueRepository(CliniqueDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        // CRUD operations
        public Task<List<Clinique>> GetAllAsync()
        {
            return _context.Cliniques.ToListAsync();
        }

        public Task<Clinique?> GetByIdAsync(Guid id)
        {
            return _context.Cliniques.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Clinique clinique)
        {
            await _context.Cliniques.AddAsync(clinique);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Clinique clinique)
        {
            _context.Cliniques.Update(clinique);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var clinique = await GetByIdAsync(id);
            if (clinique != null)
            {
                _context.Cliniques.Remove(clinique);
                await _context.SaveChangesAsync();
            }
        }

        // Search operations
        public async Task<List<Clinique?>> GetByNameAsync(string name)
        {
            return await _context.Cliniques.Where(c => c.Nom.ToLower().Contains(name)).ToListAsync();
        }

        public async Task<List<Clinique?>> GetByAddressAsync(string address)
        {
            return await _context.Cliniques.Where(c => c.Adresse.ToLower().Contains(address)).ToListAsync();
        }



        //Statistiques des cliniques
        public async Task<int> GetNombreCliniquesAsync()
        {
            return await _context.Cliniques.CountAsync();
        }

        public async Task<int> GetNombreNouvellesCliniquesDuMoisAsync()
        {
            var debutMois = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            return await _context.Cliniques.CountAsync(c => c.DateCreation >= debutMois);
        }

        public async Task<IEnumerable<Statistique>> GetNombreNouvellesCliniquesParMoisAsync()
        {
            return await _context.Cliniques
                .GroupBy(c => new { c.DateCreation.Year, c.DateCreation.Month })
                .Select(g => new Statistique
                {
                    Cle = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month)} {g.Key.Year}",
                    Nombre = g.Count()
                })
                .ToListAsync();
        }

        public async Task<StatistiqueClinique> GetStatistiquesDesCliniquesAsync(Guid cliniqueId)
        {
            var responseMedecin = await _httpClient.GetAsync($"http://localhost:5050/api/Medecin/medecinsIds/clinique/{cliniqueId}");
            if (!responseMedecin.IsSuccessStatusCode)
                throw new Exception("Erreur lors de la récupération des médecins");

            var medecinIds = await responseMedecin.Content.ReadFromJsonAsync<List<Guid>>();
            if (medecinIds == null || !medecinIds.Any())
            {
                var clinique = await _context.Cliniques.FindAsync(cliniqueId);
                return new StatistiqueClinique
                {
                    CliniqueId = cliniqueId,
                    Nom = clinique?.Nom ?? "Inconnu",
                    NombreMedecins = 0,
                    NombreConsultations = 0,
                    NombreRendezVous = 0,
                    NombrePatients = 0
                };
            }

            string queryString = string.Join("&", medecinIds.Select(id => $"medecinIds={id}"));

            var responseConsultation = await _httpClient.GetAsync($"http://localhost:5015/api/Consultation/countByMedecinIds?{queryString}");
            var responseRDV = await _httpClient.GetAsync($"http://localhost:5133/api/RendezVous/count?{queryString}");
            var responsePatient = await _httpClient.GetAsync($"http://localhost:5133/api/RendezVous/distinct/patients?{queryString}");

            if (!responseConsultation.IsSuccessStatusCode || !responseRDV.IsSuccessStatusCode || !responsePatient.IsSuccessStatusCode)
                throw new Exception("Erreur lors de la récupération des statistiques externes");

            var nbConsultations = await responseConsultation.Content.ReadFromJsonAsync<int>();
            var nbRDV = await responseRDV.Content.ReadFromJsonAsync<int>();
            var nbPatients = await responsePatient.Content.ReadFromJsonAsync<int>();

            var cliniqueNom = await _context.Cliniques
                .Where(c => c.Id == cliniqueId)
                .Select(c => c.Nom)
                .FirstOrDefaultAsync();

            return new StatistiqueClinique
            {
                CliniqueId = cliniqueId,
                Nom = cliniqueNom ?? "Inconnu",
                NombreMedecins = medecinIds.Count,
                NombreConsultations = nbConsultations,
                NombreRendezVous = nbRDV,
                NombrePatients = nbPatients
            };
        }
    }
}
