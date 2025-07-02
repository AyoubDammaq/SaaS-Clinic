using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.Interfaces;
using Clinic.Domain.ValueObject;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net.Http.Json;


namespace Clinic.Infrastructure.Repositories
{
    public class CliniqueRepository : ICliniqueRepository
    {
        private readonly CliniqueDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CliniqueRepository> _logger;

        public CliniqueRepository(CliniqueDbContext context, HttpClient httpClient, IConfiguration configuration, ILogger<CliniqueRepository> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        // CRUD operations
        public async Task<List<Clinique>> GetAllAsync()
        {
            return await _context.Cliniques.ToListAsync();
        }

        public async Task<Clinique?> GetByIdAsync(Guid id)
        {
            return await _context.Cliniques.FirstOrDefaultAsync(m => m.Id == id);
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

        public async Task<List<Clinique?>> GetByTypeAsync(TypeClinique type)
        {
            return await _context.Cliniques.Where(c => c.TypeClinique == type).ToListAsync();
        }

        public async Task<List<Clinique?>> GetByStatusAsync(StatutClinique statut)
        {
            return await _context.Cliniques.Where(c => c.Statut == statut).ToListAsync();
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
            var gatewayBaseUrl = _configuration["ServiceUrls:Gateway"];
            if (string.IsNullOrEmpty(gatewayBaseUrl))
                throw new InvalidOperationException("La configuration de l'URL du gateway est manquante.");

            // 1. Vérifier si la clinique existe
            var clinique = await _context.Cliniques.FindAsync(cliniqueId);
            if (clinique == null)
                throw new KeyNotFoundException($"Clinique avec l'ID {cliniqueId} introuvable.");

            // 2. Récupération des IDs des médecins de cette clinique
            var medecinResponse = await _httpClient.GetAsync($"{gatewayBaseUrl}/doctors/medecinsIds/clinique/{cliniqueId}");

            if (!medecinResponse.IsSuccessStatusCode)
            {
                var error = await medecinResponse.Content.ReadAsStringAsync();
                _logger.LogError("Erreur récupération médecins: {Error}", error);
                throw new HttpRequestException("Erreur lors de la récupération des médecins", null, medecinResponse.StatusCode);
            }

            var medecinIds = await medecinResponse.Content.ReadFromJsonAsync<List<Guid>>();
            if (medecinIds == null || !medecinIds.Any())
            {
                return new StatistiqueClinique
                {
                    CliniqueId = cliniqueId,
                    Nom = clinique.Nom,
                    NombreMedecins = 0,
                    NombreConsultations = 0,
                    NombreRendezVous = 0,
                    NombrePatients = 0
                };
            }

            var queryString = string.Join("&", medecinIds.Select(id => $"medecinIds={id}"));

            // 3. Exécution parallèle des requêtes
            var consultationTask = _httpClient.GetAsync($"{gatewayBaseUrl}/consultations/countByMedecinIds?{queryString}");
            var rdvTask = _httpClient.GetAsync($"{gatewayBaseUrl}/appointments/count?{queryString}");
            var patientTask = _httpClient.GetAsync($"{gatewayBaseUrl}/appointments/distinct/patients?{queryString}");

            await Task.WhenAll(consultationTask, rdvTask, patientTask);

            if (!consultationTask.Result.IsSuccessStatusCode || !rdvTask.Result.IsSuccessStatusCode || !patientTask.Result.IsSuccessStatusCode)
            {
                _logger.LogError("Une ou plusieurs requêtes ont échoué : consultations ({Status1}), rdv ({Status2}), patients ({Status3})",
                    consultationTask.Result.StatusCode,
                    rdvTask.Result.StatusCode,
                    patientTask.Result.StatusCode);

                throw new Exception("Erreur lors de la récupération des statistiques externes.");
            }

            int nbConsultations = await consultationTask.Result.Content.ReadFromJsonAsync<int>();
            int nbRDV = await rdvTask.Result.Content.ReadFromJsonAsync<int>();
            int nbPatients = await patientTask.Result.Content.ReadFromJsonAsync<int>();

            return new StatistiqueClinique
            {
                CliniqueId = cliniqueId,
                Nom = clinique.Nom,
                NombreMedecins = medecinIds.Count,
                NombreConsultations = nbConsultations,
                NombreRendezVous = nbRDV,
                NombrePatients = nbPatients
            };
        }
    }
}
