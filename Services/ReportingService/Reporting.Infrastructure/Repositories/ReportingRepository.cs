using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Reporting.Domain.Interfaces;
using Reporting.Domain.ValueObject;


namespace Reporting.Infrastructure.Repositories
{
    public class ReportingRepository : IReportingRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ReportingRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<int> GetNombreConsultationsAsync(DateTime dateDebut, DateTime dateFin)
        {
            var baseUrl = _configuration["ApiUrls:Consultation"];
            var url = $"{baseUrl}?start={dateDebut:yyyy-MM-ddTHH:mm:ss}&end={dateFin:yyyy-MM-ddTHH:mm:ss}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erreur lors de l'appel à l'API: {response.StatusCode} - {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var nbrConsultations = JsonConvert.DeserializeObject<int>(content);

            return nbrConsultations;
        }

        public async Task<IEnumerable<RendezVousStat>> GetStatistiquesRendezVousAsync(DateTime dateDebut, DateTime dateFin)
        {
            var baseUrl = _configuration["ApiUrls:RendezVous"];
            var url = $"{baseUrl}?start={dateDebut:yyyy-MM-ddTHH:mm:ss}&end={dateFin:yyyy-MM-ddTHH:mm:ss}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erreur lors de la récupération des statistiques : {response.StatusCode} - {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var allStats = JsonConvert.DeserializeObject<List<RendezVousStat>>(content);

            if (allStats == null || !allStats.Any())
            {
                throw new Exception("Aucune statistique de rendez-vous n'a été trouvée.");
            }

            return allStats;
        }

        public async Task<int> GetNombreNouveauxPatientsAsync(DateTime dateDebut, DateTime dateFin)
        {
            var baseUrl = _configuration["ApiUrls:Patients"];
            var url = $"{baseUrl}?start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return int.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<List<DoctorStats>> GetNombreMedecinParSpecialiteAsync()
        {
            var url = _configuration["ApiUrls:MedecinSpecialite"];
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DoctorStats>>(content);
        }

        public async Task<List<DoctorStats>> GetNombreMedecinByCliniqueAsync()
        {
            var url = _configuration["ApiUrls:MedecinClinique"];
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DoctorStats>>(content);
        }

        public async Task<List<DoctorStats>> GetNombreMedecinBySpecialiteDansUneCliniqueAsync(Guid cliniqueId)
        {
            var baseUrl = _configuration["ApiUrls:MedecinSpecialiteClinique"];
            var url = $"{baseUrl}/{cliniqueId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DoctorStats>>(content);
        }

        public async Task<List<FactureStats>> GetNombreDeFactureByStatusAsync()
        {
            var url = _configuration["ApiUrls:FactureStatus"];
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FactureStats>>(content);
        }

        public async Task<List<FactureStats>> GetNombreDeFactureParCliniqueAsync()
        {
            var url = _configuration["ApiUrls:FactureClinic"];
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FactureStats>>(content);
        }

        public async Task<List<FactureStats>> GetNombreDeFacturesByStatusParCliniqueAsync()
        {
            var url = _configuration["ApiUrls:FactureStatusClinic"];
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FactureStats>>(content);
        }

        public async Task<List<FactureStats>> GetNombreDeFacturesByStatusDansUneCliniqueAsync(Guid cliniqueId)
        {
            var baseUrl = _configuration["ApiUrls:FactureStatusClinic"];
            var url = $"{baseUrl}/{cliniqueId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FactureStats>>(content);
        }

        public async Task<int> GetNombreDeCliniquesAsync()
        {
            var url = _configuration["ApiUrls:CliniqueNombre"];
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return int.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<int> GetNombreNouvellesCliniquesDuMoisAsync()
        {
            var url = _configuration["ApiUrls:CliniqueNouvellesMois"];
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return int.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<IEnumerable<Statistique>> GetNombreNouvellesCliniquesParMoisAsync()
        {
            var url = _configuration["ApiUrls:CliniqueNouvellesParMois"];
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Statistique>>(content);
        }

        public async Task<StatistiqueClinique> GetStatistiquesCliniqueAsync(Guid cliniqueId)
        {
            var baseUrl = _configuration["ApiUrls:CliniqueStats"];
            var url = $"{baseUrl}/{cliniqueId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatistiqueClinique>(content);
        }

        public async Task<IEnumerable<ActiviteMedecin>> GetActivitesMedecinAsync(Guid medecinId)
        {
            var baseUrl = _configuration["ApiUrls:MedecinActivites"];
            var url = $"{baseUrl}/{medecinId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ActiviteMedecin>>(content);
        }

        public async Task<decimal> GetMontantPaiementsAsync(string statut, DateTime dateDebut, DateTime dateFin)
        {
            var baseUrl = _configuration["ApiUrls:PaiementMontant"];
            var url = $"{baseUrl}?statut={statut}&start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return decimal.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<StatistiquesFacture> GetStatistiquesFacturesAsync(DateTime dateDebut, DateTime dateFin)
        {
            var baseUrl = _configuration["ApiUrls:FactureStatistiques"];
            var url = $"{baseUrl}?debut={dateDebut:yyyy-MM-dd}&fin={dateFin:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatistiquesFacture>(content);
        }


        public async Task<DashboardStats> GetDashboardStatsAsync(DateTime dateDebut, DateTime dateFin)
        {
            var statsFacture = await GetStatistiquesFacturesAsync(dateDebut, dateFin);

            return new DashboardStats
            {
                ConsultationsJour = await GetNombreConsultationsAsync(dateDebut, dateFin),
                NouveauxPatients = await GetNombreNouveauxPatientsAsync(dateDebut, dateFin),
                NombreFactures = statsFacture.NombreTotal,
                TotalFacturesPayees = statsFacture.NombrePayees,
                TotalFacturesImpayees = statsFacture.NombreImpayees,
                PaiementsPayes = statsFacture.MontantTotalPaye,
                PaiementsImpayes = statsFacture.MontantTotal - statsFacture.MontantTotalPaye,
                PaiementsEnAttente = statsFacture.NombrePartiellementPayees
            };
        }
    }
}

