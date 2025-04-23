using Newtonsoft.Json;
using Reporting.Domain.Interfaces;
using Reporting.Domain.ValueObject;


namespace Reporting.Infrastructure.Repositories
{
    public class ReportingRepository : IReportingRepository
    {
        private readonly HttpClient _httpClient;

        public ReportingRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> GetNombreConsultationsAsync(DateTime dateDebut, DateTime dateFin)
        {
            var url = $"http://localhost:5015/api/Consultation/count?start={dateDebut:yyyy-MM-ddTHH:mm:ss}&end={dateFin:yyyy-MM-ddTHH:mm:ss}";
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
            var url = $"http://localhost:5133/api/RendezVous/period?start={dateDebut:yyyy-MM-ddTHH:mm:ss}&end={dateFin:yyyy-MM-ddTHH:mm:ss}"; 
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
            var url = $"http://localhost:5269/api/Patients/statistiques?start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return int.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<List<DoctorStats>> GetNombreMedecinParSpecialiteAsync()
        {
            var url = "http://localhost:5050/api/Medecin/statistiques/specialite";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DoctorStats>>(content);
        }


        public async Task<List<DoctorStats>> GetNombreMedecinByCliniqueAsync()
        {
            var url = "http://localhost:5050/api/Medecin/statistiques/clinique";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DoctorStats>>(content);
        }

        public async Task<List<DoctorStats>> GetNombreMedecinBySpecialiteDansUneCliniqueAsync(Guid cliniqueId)
        {
            var url = $"http://localhost:5050/api/Medecin/statistiques/specialite/clinique/{cliniqueId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DoctorStats>>(content);
        }

        public async Task<List<FactureStats>> GetNombreDeFactureByStatusAsync()
        {
            var url = "http://localhost:5135/api/Facture/stats/status";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FactureStats>>(content);
        }

        public async Task<List<FactureStats>> GetNombreDeFactureParCliniqueAsync()
        {
            var url = "http://localhost:5135/api/Facture/stats/clinic";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FactureStats>>(content);
        }

        public async Task<List<FactureStats>> GetNombreDeFacturesByStatusParCliniqueAsync()
        {
            var url = "http://localhost:5135/api/Facture/stats/status/clinic";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FactureStats>>(content);
        }

        public async Task<List<FactureStats>> GetNombreDeFacturesByStatusDansUneCliniqueAsync(Guid cliniqueId)
        {
            var url = $"http://localhost:5135/api/Facture/stats/status/clinic/{cliniqueId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FactureStats>>(content);
        }

        public async Task<int> GetNombreDeCliniquesAsync()
        {
            var url = "http://localhost:5210/api/Clinique/nombre-cliniques"; 
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return int.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<int> GetNombreNouvellesCliniquesDuMoisAsync()
        {
            var url = "http://localhost:5210/api/Clinique/nouvelles-cliniques-mois";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return int.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<IEnumerable<Statistique>> GetNombreNouvellesCliniquesParMoisAsync()
        {
            var url = "http://localhost:5210/api/Clinique/nouvelles-cliniques-par-mois";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Statistique>>(content);
        }

        public async Task<StatistiqueClinique> GetStatistiquesCliniqueAsync(Guid cliniqueId)
        {
            var url = $"http://localhost:5210/api/Clinique/statistiques/{cliniqueId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatistiqueClinique>(content);
        }


        public async Task<IEnumerable<ActiviteMedecin>> GetActivitesMedecinAsync(Guid medecinId)
        {
            var url = $"http://localhost:5050/api/Medecin/activites/{medecinId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ActiviteMedecin>>(content);
        }











        public async Task<decimal> GetMontantPaiementsAsync(string statut, DateTime dateDebut, DateTime dateFin)
        {
            var url = $"https://paiement-api/api/paiements/montant?statut={statut}&start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return decimal.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<int> GetNombreFacturesAsync(DateTime dateDebut, DateTime dateFin)
        {
            var url = $"https://facture-api/api/factures/count?start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return int.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<decimal> GetMontantFacturesAsync(string statut, DateTime dateDebut, DateTime dateFin)
        {
            var url = $"https://facture-api/api/factures/montant?statut={statut}&start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return decimal.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<DashboardStats> GetDashboardStatsAsync(DateTime dateDebut, DateTime dateFin)
        {
            return new DashboardStats
            {
                ConsultationsJour = await GetNombreConsultationsAsync(dateDebut, dateFin),
                NouveauxPatients = await GetNombreNouveauxPatientsAsync(dateDebut, dateFin),
                NombreFactures = await GetNombreFacturesAsync(dateDebut, dateFin),
                TotalFacturesPayees = await GetMontantFacturesAsync("payee", dateDebut, dateFin),
                TotalFacturesImpayees = await GetMontantFacturesAsync("impayee", dateDebut, dateFin),
                PaiementsPayes = await GetMontantPaiementsAsync("payee", dateDebut, dateFin),
                PaiementsImpayes = await GetMontantPaiementsAsync("impayee", dateDebut, dateFin),
                PaiementsEnAttente = await GetMontantPaiementsAsync("en_attente", dateDebut, dateFin)
            };
        }
    }
}

