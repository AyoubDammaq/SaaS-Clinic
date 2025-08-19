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

        public async Task<int> GetNombreDeNouvellesCliniquesAsync(DateTime dateDebut, DateTime dateFin)
        {
            var baseUrl = _configuration["ApiUrls:NombreDeNouvellesCliniquesParMois"];
            var url = $"{baseUrl}?start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return int.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<int> GetNombreNouveauxMedecinsAsync(DateTime dateDebut, DateTime dateFin)
        {
            var baseUrl = _configuration["ApiUrls:NouveauxMedecins"];
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

        public async Task<IEnumerable<AppointmentDayStat>> GetStatistiquesHebdomadairesRendezVousByDoctorAsync(Guid medecinId, DateTime dateDebut, DateTime dateFin)
        {
            var baseUrl = _configuration["ApiUrls:RendezVousHebdomadaireByDoctor"];
            var url = $"{baseUrl}/{medecinId}?start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erreur lors de la récupération des statistiques hebdomadaires des rendez-vous : {response.StatusCode} - {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var stats = JsonConvert.DeserializeObject<List<AppointmentDayStat>>(content);

            return stats;
        }

        public async Task<IEnumerable<AppointmentDayStat>> GetStatistiquesHebdomadairesRendezVousByClinicAsync(Guid cliniqueId, DateTime dateDebut, DateTime dateFin)
        {
            var baseUrl = _configuration["ApiUrls:RendezVousHebdomadaireByClinic"];
            var url = $"{baseUrl}/{cliniqueId}?start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erreur lors de la récupération des statistiques hebdomadaires des rendez-vous : {response.StatusCode} - {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var stats = JsonConvert.DeserializeObject<List<AppointmentDayStat>>(content);

            return stats;
        }

        public async Task<(int trendValue, bool isPositive)> CalculerTrendDeConsultationsMensuellesParDoctorAsync(Guid medecinId)
        {
            if (medecinId == Guid.Empty)
                throw new ArgumentException("medecinId ne peut pas être vide", nameof(medecinId));

            // Déterminer le mois courant et le mois précédent
            var now = DateTime.UtcNow;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddTicks(-1);

            var prevMonthStart = currentMonthStart.AddMonths(-1);
            var prevMonthEnd = currentMonthStart.AddTicks(-1);

            // Appeler l'endpoint pour le mois courant
            var currentCountResponse = await _httpClient.GetAsync(
                $"http://consultationservice:8091/api/Consultation/count-consultation-by-doctor/{medecinId}?startDate={currentMonthStart:yyyy-MM-dd}&endDate={currentMonthEnd:yyyy-MM-dd}"
            );
            currentCountResponse.EnsureSuccessStatusCode();
            var currentCount = int.Parse(await currentCountResponse.Content.ReadAsStringAsync());

            // Appeler l'endpoint pour le mois précédent
            var prevCountResponse = await _httpClient.GetAsync(
                $"http://consultationservice:8091/api/Consultation/count-consultation-by-doctor/{medecinId}?startDate={prevMonthStart:yyyy-MM-dd}&endDate={prevMonthEnd:yyyy-MM-dd}"
            );
            prevCountResponse.EnsureSuccessStatusCode();
            var prevCount = int.Parse(await prevCountResponse.Content.ReadAsStringAsync());

            // Calcul du trend
            int trendValue;
            if (prevCount > 0)
                trendValue = (int)Math.Round(((double)(currentCount - prevCount) / prevCount) * 100);
            else
                trendValue = currentCount > 0 ? 100 : 0;

            var isPositive = currentCount >= prevCount;

            return (trendValue, isPositive);
        }

        public async Task<(int trendValue, bool isPositive)> CalculerTrendDeConsultationsMensuellesParClinicAsync(Guid cliniqueId)
        {
            if (cliniqueId == Guid.Empty)
                throw new ArgumentException("cliniqueId ne peut pas être vide", nameof(cliniqueId));
            // Déterminer le mois courant et le mois précédent
            var now = DateTime.UtcNow;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddTicks(-1);
            var prevMonthStart = currentMonthStart.AddMonths(-1);
            var prevMonthEnd = currentMonthStart.AddTicks(-1);
            // Appeler l'endpoint pour le mois courant
            var currentCountResponse = await _httpClient.GetAsync(
                $"http://consultationservice:8091/api/Consultation/count-consultation-by-clinic/{cliniqueId}?startDate={currentMonthStart:yyyy-MM-dd}&endDate={currentMonthEnd:yyyy-MM-dd}"
            );
            currentCountResponse.EnsureSuccessStatusCode();
            var currentCount = int.Parse(await currentCountResponse.Content.ReadAsStringAsync());
            // Appeler l'endpoint pour le mois précédent
            var prevCountResponse = await _httpClient.GetAsync(
                $"http://consultationservice:8091/api/Consultation/count-consultation-by-clinic/{cliniqueId}?startDate={prevMonthStart:yyyy-MM-dd}&endDate={prevMonthEnd:yyyy-MM-dd}"
            );
            prevCountResponse.EnsureSuccessStatusCode();
            var prevCount = int.Parse(await prevCountResponse.Content.ReadAsStringAsync());
            // Calcul du trend
            int trendValue;
            if (prevCount > 0)
                trendValue = (int)Math.Round(((double)(currentCount - prevCount) / prevCount) * 100);
            else
                trendValue = currentCount > 0 ? 100 : 0;
            var isPositive = currentCount >= prevCount;
            return (trendValue, isPositive);
        }

        public async Task<Trend> CalculerTrendDeNouveauxPatientsMensuelsParMedecinAsync(Guid medecinId)
        {
            if (medecinId == Guid.Empty)
                throw new ArgumentException("medecinId ne peut pas être vide", nameof(medecinId));

            var now = DateTime.UtcNow;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddTicks(-1);
            var prevMonthStart = currentMonthStart.AddMonths(-1);
            var prevMonthEnd = currentMonthStart.AddTicks(-1);

            // Appeler l'API pour le mois courant
            var currentCountResponse = await _httpClient.GetAsync(
                $"http://consultationservice:8091/api/Consultation/nouveaux-patients-count-by-doctor/{medecinId}?startDate={currentMonthStart:yyyy-MM-dd}&endDate={currentMonthEnd:yyyy-MM-dd}"
            );
            currentCountResponse.EnsureSuccessStatusCode();
            var currentCount = int.Parse(await currentCountResponse.Content.ReadAsStringAsync());

            // Appeler l'API pour le mois précédent
            var prevCountResponse = await _httpClient.GetAsync(
                $"http://consultationservice:8091/api/Consultation/nouveaux-patients-count-by-doctor/{medecinId}?startDate={prevMonthStart:yyyy-MM-dd}&endDate={prevMonthEnd:yyyy-MM-dd}"
            );
            prevCountResponse.EnsureSuccessStatusCode();
            var prevCount = int.Parse(await prevCountResponse.Content.ReadAsStringAsync());

            // Calcul du trend
            if (prevCount == 0 && currentCount == 0)
                return new Trend { Value = 0, IsPositive = true };

            if (prevCount == 0)
                return new Trend { Value = 100, IsPositive = true };

            var delta = currentCount - prevCount;
            var percentage = Math.Abs((int)Math.Round((double)delta / prevCount * 100));

            return new Trend
            {
                Value = percentage,
                IsPositive = delta >= 0
            };
        }

        public async Task<Trend> CalculerTrendDeNouveauxPatientsMensuelsParCliniqueAsync(Guid cliniqueId)
        {
            if (cliniqueId == Guid.Empty)
                throw new ArgumentException("cliniqueId ne peut pas être vide", nameof(cliniqueId));

            var now = DateTime.UtcNow;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddTicks(-1);
            var prevMonthStart = currentMonthStart.AddMonths(-1);
            var prevMonthEnd = currentMonthStart.AddTicks(-1);

            // Appeler l'API pour le mois courant
            var currentCountResponse = await _httpClient.GetAsync(
                $"http://consultationservice:8091/api/Consultation/nouveaux-patients-count-by-clinic/{cliniqueId}?startDate={currentMonthStart:yyyy-MM-dd}&endDate={currentMonthEnd:yyyy-MM-dd}"
            );
            currentCountResponse.EnsureSuccessStatusCode();
            var currentCount = int.Parse(await currentCountResponse.Content.ReadAsStringAsync());

            // Appeler l'API pour le mois précédent
            var prevCountResponse = await _httpClient.GetAsync(
                $"http://consultationservice:8091/api/Consultation/nouveaux-patients-count-by-clinic/{cliniqueId}?startDate={prevMonthStart:yyyy-MM-dd}&endDate={prevMonthEnd:yyyy-MM-dd}"
            );
            prevCountResponse.EnsureSuccessStatusCode();
            var prevCount = int.Parse(await prevCountResponse.Content.ReadAsStringAsync());

            // Calcul du trend
            if (prevCount == 0 && currentCount == 0)
                return new Trend { Value = 0, IsPositive = true };

            if (prevCount == 0)
                return new Trend { Value = 100, IsPositive = true };

            var delta = currentCount - prevCount;
            var percentage = Math.Abs((int)Math.Round((double)delta / prevCount * 100));

            return new Trend
            {
                Value = percentage,
                IsPositive = delta >= 0
            };
        }

        public async Task<Trend> CalculerNewClinicsTrend(DateTime dateDebut, DateTime dateFin)
        {
            if (dateDebut > dateFin)
                throw new ArgumentException("La date de début doit être antérieure à la date de fin");

            // Déterminer les périodes : mois courant et mois précédent
            var currentStart = new DateTime(dateDebut.Year, dateDebut.Month, 1);
            var currentEnd = currentStart.AddMonths(1).AddTicks(-1);

            var prevStart = currentStart.AddMonths(-1);
            var prevEnd = currentStart.AddTicks(-1);

            // Appeler l'API pour le mois courant
            var currentCount = await GetNombreDeNouvellesCliniquesAsync(currentStart, currentEnd);

            // Appeler l'API pour le mois précédent
            var prevCount = await GetNombreDeNouvellesCliniquesAsync(prevStart, prevEnd);

            // Calcul du trend
            if (prevCount == 0 && currentCount == 0)
                return new Trend { Value = 0, IsPositive = true };

            if (prevCount == 0)
                return new Trend { Value = 100, IsPositive = true };

            var delta = currentCount - prevCount;
            var percentage = Math.Abs((int)Math.Round((double)delta / prevCount * 100));

            return new Trend
            {
                Value = percentage,
                IsPositive = delta >= 0
            };
        }

        public async Task<Trend> CalculerNewDoctorsTrend(DateTime dateDebut, DateTime dateFin)
        {
            if (dateDebut > dateFin)
                throw new ArgumentException("La date de début doit être antérieure à la date de fin");

            // Déterminer les périodes : mois courant et mois précédent
            var currentStart = new DateTime(dateDebut.Year, dateDebut.Month, 1);
            var currentEnd = currentStart.AddMonths(1).AddTicks(-1);

            var prevStart = currentStart.AddMonths(-1);
            var prevEnd = currentStart.AddTicks(-1);

            // Appeler l'API pour le mois courant
            var currentCount = await GetNombreNouveauxMedecinsAsync(currentStart, currentEnd);

            // Appeler l'API pour le mois précédent
            var prevCount = await GetNombreNouveauxMedecinsAsync(prevStart, prevEnd);

            // Calcul du trend
            if (prevCount == 0 && currentCount == 0)
                return new Trend { Value = 0, IsPositive = true };

            if (prevCount == 0)
                return new Trend { Value = 100, IsPositive = true };

            var delta = currentCount - prevCount;
            var percentage = Math.Abs((int)Math.Round((double)delta / prevCount * 100));

            return new Trend
            {
                Value = percentage,
                IsPositive = delta >= 0
            };
        }

        public async Task<Trend> CalculerNewPatientsTrend(DateTime dateDebut, DateTime dateFin)
        {
            if (dateDebut > dateFin)
                throw new ArgumentException("La date de début doit être antérieure à la date de fin");
            // Déterminer les périodes : mois courant et mois précédent
            var currentStart = new DateTime(dateDebut.Year, dateDebut.Month, 1);
            var currentEnd = currentStart.AddMonths(1).AddTicks(-1);
            var prevStart = currentStart.AddMonths(-1);
            var prevEnd = currentStart.AddTicks(-1);
            // Appeler l'API pour le mois courant
            var currentCount = await GetNombreNouveauxPatientsAsync(currentStart, currentEnd);
            // Appeler l'API pour le mois précédent
            var prevCount = await GetNombreNouveauxPatientsAsync(prevStart, prevEnd);
            // Calcul du trend
            if (prevCount == 0 && currentCount == 0)
                return new Trend { Value = 0, IsPositive = true };
            if (prevCount == 0)
                return new Trend { Value = 100, IsPositive = true };
            var delta = currentCount - prevCount;
            var percentage = Math.Abs((int)Math.Round((double)delta / prevCount * 100));
            return new Trend
            {
                Value = percentage,
                IsPositive = delta >= 0
            };
        }



        public async Task<int> CalculerNombreDeRDVParMedecinAujourdhuiAsync(Guid medecinId)
        {
            if (medecinId == Guid.Empty)
                throw new ArgumentException("medecinId ne peut pas être vide", nameof(medecinId));
            var today = DateTime.UtcNow.Date;
            var url = $"{_configuration["ApiUrls:RendezVousCountByDoctor"]}/{medecinId}?date={today:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return int.Parse(await response.Content.ReadAsStringAsync());
        }


        public async Task<DashboardStats> GetDashboardStatsAsync(DateTime dateDebut, DateTime dateFin, Guid? patientId = null, Guid? medecinId = null, Guid? cliniqueId = null)
        {
            var stats = new DashboardStats
            {
                TrendDeConsultationsMensuellesParDoctor = new Trend(), // Initialize Trend to avoid null reference
                TrendDeConsultationsMensuellesParClinic = new Trend(),  // Initialize Trend to avoid null reference
                TrendDeNouveauxPatientsMensuelsParMedecin = new Trend(), // Initialize Trend to avoid null reference
                TrendDeNouveauxPatientsMensuelsParClinic = new Trend(), // Initialize Trend to avoid null reference
                TrendDeNouveauxMedecins = new Trend(), // Initialize Trend to avoid null reference
                TrendDeNouvellesCliniques = new Trend(), // Initialize Trend to avoid null reference
                TrendDeNouveauxPatients = new Trend() // Initialize Trend to avoid null reference
            };

            // Récupérer les statistiques de facturation
            var statsFacture = await GetStatistiquesFacturesAsync(dateDebut, dateFin);
            stats.NombreFactures = statsFacture.NombreTotal;
            stats.TotalFacturesPayees = statsFacture.NombrePayees;
            stats.TotalFacturesImpayees = statsFacture.NombreImpayees;
            stats.PaiementsPayes = statsFacture.MontantTotalPaye;
            stats.PaiementsImpayes = statsFacture.MontantTotal - statsFacture.MontantTotalPaye;
            stats.PaiementsEnAttente = statsFacture.NombrePartiellementPayees;

            var totalPatientsUrl = _configuration["ApiUrls:TotalPatients"];
            var totalPatientsResponse = await _httpClient.GetAsync(totalPatientsUrl);
            totalPatientsResponse.EnsureSuccessStatusCode();
            stats.TotalPatients = int.Parse(await totalPatientsResponse.Content.ReadAsStringAsync());

            // Récupérer le nombre de consultations
            var queryParams = new List<string>();

            if (cliniqueId.HasValue)
                queryParams.Add($"cliniqueId={cliniqueId}");
            if (medecinId.HasValue)
                queryParams.Add($"medecinId={medecinId}");
            if (patientId.HasValue)
                queryParams.Add($"patientId={patientId}");

            queryParams.Add($"dateDebut={dateDebut:O}");
            queryParams.Add($"dateFin={dateFin:O}");

            var consultationUrl = $"{_configuration["ApiUrls:ConsultationByClinicByDate"]}?{string.Join("&", queryParams)}";
            var consultationResponse = await _httpClient.GetAsync(consultationUrl);
            consultationResponse.EnsureSuccessStatusCode();
            stats.ConsultationsJour = int.Parse(await consultationResponse.Content.ReadAsStringAsync());

            // Récupérer le nombre de nouveaux patients (seulement si patientId n'est PAS défini)
            if (!patientId.HasValue)
            {
                if (medecinId.HasValue || cliniqueId.HasValue)
                {
                    var patientUrl = _configuration["ApiUrls:NouveauxPatients"];

                    if (medecinId.HasValue)
                        patientUrl += $"-by-doctor/{medecinId}";
                    else if (cliniqueId.HasValue)
                        patientUrl += $"-by-clinic/{cliniqueId}";

                    patientUrl += $"?startDate={dateDebut:yyyy-MM-dd}&endDate={dateFin:yyyy-MM-dd}";

                    var patientResponse = await _httpClient.GetAsync(patientUrl);
                    patientResponse.EnsureSuccessStatusCode();

                    stats.NouveauxPatients = int.Parse(await patientResponse.Content.ReadAsStringAsync());
                }
                else
                {
                    stats.NouveauxPatients = await GetNombreNouveauxPatientsAsync(dateDebut, dateFin);
                }
            }

            // Récupérer les nouveaux patients par mois (seulement si patientId n'est PAS défini)
            if (!patientId.HasValue)
            {
                var dateActuel = DateTime.Now;
                var newPatientsByMonthUrl = $"{_configuration["ApiUrls:NouveauxPatientsParMois"]}?dateActuel={dateActuel:yyyy-MM-dd}";
                var newPatientsByMonthResponse = await _httpClient.GetAsync(newPatientsByMonthUrl);
                newPatientsByMonthResponse.EnsureSuccessStatusCode();
                var content = await newPatientsByMonthResponse.Content.ReadAsStringAsync();
                var dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(content);
                // Si tu as besoin de la liste StatistiqueDTO, convertis ce dictionnaire :
                stats.NouveauxPatientsParMois = dict.Select(kvp => new Statistique
                {
                    Cle = kvp.Key,
                    Nombre = kvp.Value
                }).ToList();
            }

            // Récupérer le nombre de médecins
            if (!patientId.HasValue && !medecinId.HasValue && !cliniqueId.HasValue)
            {
                var medecinUrl = _configuration["ApiUrls:TotalMedecins"];
                var medecinResponse = await _httpClient.GetAsync(medecinUrl);
                medecinResponse.EnsureSuccessStatusCode();
                stats.TotalMedecins = int.Parse(await medecinResponse.Content.ReadAsStringAsync());

                var newMedecinUrl = _configuration["ApiUrls:NouveauxMedecins"];
                var newMedecinResponse = await _httpClient.GetAsync(newMedecinUrl);
                newMedecinResponse.EnsureSuccessStatusCode();
                stats.NouveauxMedecins = int.Parse(await newMedecinResponse.Content.ReadAsStringAsync());
            }

            bool isSuperAdmin = !patientId.HasValue && !medecinId.HasValue && !cliniqueId.HasValue;

            // Récupérer les statistiques de rendez-vous
            if (!patientId.HasValue)
            {
                string rdvUrl = null;

                if (isSuperAdmin)
                {
                    // SuperAdmin → endpoint global
                    rdvUrl = _configuration["ApiUrls:RendezVous"];
                }
                else if (medecinId.HasValue)
                {
                    rdvUrl = $"{_configuration["ApiUrls:RendezVousParMedecinParPeriod"]}/{medecinId}";
                }
                else if (cliniqueId.HasValue)
                {
                    rdvUrl = $"{_configuration["ApiUrls:RendezVousParCliniqueParPeriod"]}/{cliniqueId}";
                }

                if (!string.IsNullOrEmpty(rdvUrl))
                {
                    rdvUrl += $"?start={dateDebut:yyyy-MM-ddTHH:mm:ss}&end={dateFin:yyyy-MM-ddTHH:mm:ss}";

                    var rdvResponse = await _httpClient.GetAsync(rdvUrl);
                    if (rdvResponse.IsSuccessStatusCode)
                    {
                        var rdvContent = await rdvResponse.Content.ReadAsStringAsync();
                        stats.RendezvousStats = JsonConvert.DeserializeObject<List<RendezVousStat>>(rdvContent);
                    }
                }
            }

            // Récupérer les médecins par spécialité (uniquement pour SuperAdmin)
            if (!patientId.HasValue && !medecinId.HasValue && !cliniqueId.HasValue)
            {
                var doctorStatsUrl = _configuration["ApiUrls:MedecinSpecialite"];
                var doctorStatsResponse = await _httpClient.GetAsync(doctorStatsUrl);
                doctorStatsResponse.EnsureSuccessStatusCode();
                var doctorStatsContent = await doctorStatsResponse.Content.ReadAsStringAsync();
                stats.DoctorsBySpecialty = JsonConvert.DeserializeObject<List<DoctorStats>>(doctorStatsContent);
            }
            else if (cliniqueId.HasValue)
            {
                var doctorStatsUrl = $"{_configuration["ApiUrls:MedecinSpecialiteClinique"]}/{cliniqueId}";
                var doctorStatsResponse = await _httpClient.GetAsync(doctorStatsUrl);
                doctorStatsResponse.EnsureSuccessStatusCode();
                var doctorStatsContent = await doctorStatsResponse.Content.ReadAsStringAsync();
                stats.DoctorsBySpecialty = JsonConvert.DeserializeObject<List<DoctorStats>>(doctorStatsContent);
            }

            // Récupérer le nombre total de cliniques (uniquement pour SuperAdmin)
            if (!patientId.HasValue && !medecinId.HasValue && !cliniqueId.HasValue)
            {
                var clinicCountUrl = _configuration["ApiUrls:CliniqueNombre"];
                var clinicCountResponse = await _httpClient.GetAsync(clinicCountUrl);
                clinicCountResponse.EnsureSuccessStatusCode();
                stats.TotalClinics = int.Parse(await clinicCountResponse.Content.ReadAsStringAsync());

                var newClinicsUrl = _configuration["ApiUrls:NouvelleCliniques"];
                var newClinicsResponse = await _httpClient.GetAsync(newClinicsUrl);
                newClinicsResponse.EnsureSuccessStatusCode();
                stats.NouvellesCliniques = int.Parse(await newClinicsResponse.Content.ReadAsStringAsync());
            }

            // Récupérer les statistiques hebdomadaires des rendez-vous par médecin
            if (medecinId.HasValue)
            {
                var weeklyDoctorUrl = $"{_configuration["ApiUrls:RendezVousHebdomadaireByDoctor"]}/{medecinId}?start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
                var weeklyDoctorResponse = await _httpClient.GetAsync(weeklyDoctorUrl);
                if (weeklyDoctorResponse.IsSuccessStatusCode)
                {
                    var weeklyDoctorContent = await weeklyDoctorResponse.Content.ReadAsStringAsync();
                    stats.WeeklyAppointmentStatsByDoctor = JsonConvert.DeserializeObject<List<AppointmentDayStat>>(weeklyDoctorContent);
                }
            }

            // Récupérer les statistiques hebdomadaires des rendez-vous par clinique
            if (cliniqueId.HasValue)
            {
                var weeklyClinicUrl = $"{_configuration["ApiUrls:RendezVousHebdomadaireByClinic"]}/{cliniqueId}?start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
                var weeklyClinicResponse = await _httpClient.GetAsync(weeklyClinicUrl);
                if (weeklyClinicResponse.IsSuccessStatusCode)
                {
                    var weeklyClinicContent = await weeklyClinicResponse.Content.ReadAsStringAsync();
                    stats.WeeklyAppointmentStatsByClinic = JsonConvert.DeserializeObject<List<AppointmentDayStat>>(weeklyClinicContent);
                }
            }

            if (patientId.HasValue)
            {
                // Récupérer le nombre de consultations par patient
                var consultationsByPatientUrl = $"{_configuration["ApiUrls:NombreDeConsultationsParPatient"]}/{patientId}?start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
                var consultationsByPatientResponse = await _httpClient.GetAsync(consultationsByPatientUrl);
                consultationsByPatientResponse.EnsureSuccessStatusCode();
                stats.NombreDeConsultationsParPatient = int.Parse(await consultationsByPatientResponse.Content.ReadAsStringAsync());

                // Récupérer le paiement récent par patient
                var recentPaymentUrl = $"{_configuration["ApiUrls:RecentPaiementByPatient"]}/{patientId}";
                var recentPaymentResponse = await _httpClient.GetAsync(recentPaymentUrl);
                if (recentPaymentResponse.IsSuccessStatusCode)
                {
                    var recentPaymentContent = await recentPaymentResponse.Content.ReadAsStringAsync();
                    stats.RecentPaiementByPatient = JsonConvert.DeserializeObject<RecentPaiement>(recentPaymentContent);
                }
            }

            if (medecinId.HasValue)
            {
                // Récupérer le nombre de nouveaux patients par médecin
                var newPatientsByDoctorUrl = $"{_configuration["ApiUrls:NouveauxPatientsParMedecin"]}/{medecinId}?startDate={dateDebut:yyyy-MM-ddTHH:mm:ssZ}&endDate={dateFin:yyyy-MM-ddTHH:mm:ssZ}";
                var newPatientsByDoctorResponse = await _httpClient.GetAsync(newPatientsByDoctorUrl);
                newPatientsByDoctorResponse.EnsureSuccessStatusCode();
                stats.NouveauxPatientsParMedecin = int.Parse(await newPatientsByDoctorResponse.Content.ReadAsStringAsync());

                var Trend = await CalculerTrendDeNouveauxPatientsMensuelsParMedecinAsync(medecinId.Value);
                stats.TrendDeNouveauxPatientsMensuelsParMedecin.Value = Trend.Value;
                stats.TrendDeNouveauxPatientsMensuelsParMedecin.IsPositive = Trend.IsPositive;
            }

            if (cliniqueId.HasValue)
            {
                // Récupérer le nombre de nouveaux patients par clinique
                var newPatientsByClinicUrl = $"{_configuration["ApiUrls:NouveauxPatientsParClinic"]}/{cliniqueId.ToString().ToLower()}?startDate={dateDebut:yyyy-MM-ddTHH:mm:ssZ}&endDate={dateFin:yyyy-MM-ddTHH:mm:ssZ}"; ;
                var newPatientsByClinicResponse = await _httpClient.GetAsync(newPatientsByClinicUrl);
                newPatientsByClinicResponse.EnsureSuccessStatusCode();
                stats.NouveauxPatientsParClinic = int.Parse(await newPatientsByClinicResponse.Content.ReadAsStringAsync());

                var Trend = await CalculerTrendDeNouveauxPatientsMensuelsParCliniqueAsync(cliniqueId.Value);
                stats.TrendDeNouveauxPatientsMensuelsParClinic.Value = Trend.Value;
                stats.TrendDeNouveauxPatientsMensuelsParClinic.IsPositive = Trend.IsPositive;
            }

            if (medecinId.HasValue)
            {
                // Récupérer le nombre de consultations mensuelles par médecin
                var monthlyConsultationsByDoctorUrl = $"{_configuration["ApiUrls:NombreDeConsultationsMensuellesParDoctor"]}/{medecinId}?start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
                var monthlyConsultationsByDoctorResponse = await _httpClient.GetAsync(monthlyConsultationsByDoctorUrl);
                monthlyConsultationsByDoctorResponse.EnsureSuccessStatusCode();
                stats.NombreDeConsultationsMensuellesParDoctor = int.Parse(await monthlyConsultationsByDoctorResponse.Content.ReadAsStringAsync());

                var (trendValue, isPositive) = await CalculerTrendDeConsultationsMensuellesParDoctorAsync(medecinId.Value);
                stats.TrendDeConsultationsMensuellesParDoctor.Value = trendValue;
                stats.TrendDeConsultationsMensuellesParDoctor.IsPositive = isPositive;
            }

            if (cliniqueId.HasValue)
            {
                // Récupérer le nombre de consultations mensuelles par clinique
                var monthlyConsultationsByClinicUrl = $"{_configuration["ApiUrls:NombreDeConsultationsMensuellesParClinique"]}/{cliniqueId}?start={dateDebut:yyyy-MM-dd}&end={dateFin:yyyy-MM-dd}";
                var monthlyConsultationsByClinicResponse = await _httpClient.GetAsync(monthlyConsultationsByClinicUrl);
                monthlyConsultationsByClinicResponse.EnsureSuccessStatusCode();
                stats.NombreDeConsultationsMensuellesParClinic = int.Parse(await monthlyConsultationsByClinicResponse.Content.ReadAsStringAsync());

                var (trendValue, isPositive) = await CalculerTrendDeConsultationsMensuellesParClinicAsync(cliniqueId.Value);
                stats.TrendDeConsultationsMensuellesParClinic.Value = trendValue;
                stats.TrendDeConsultationsMensuellesParClinic.IsPositive = isPositive;
            }

            if (medecinId.HasValue)
            {
                var nombreDeRDVParMedecinAujourdHuiUrl = $"{_configuration["ApiUrls:NombreDeRDVParMedecin"]}/{medecinId}?date={DateTime.UtcNow:yyyy-MM-dd}";
                var nombreDeRDVParMedecinAujourdHuiResponse = await _httpClient.GetAsync(nombreDeRDVParMedecinAujourdHuiUrl);
                nombreDeRDVParMedecinAujourdHuiResponse.EnsureSuccessStatusCode();
                stats.NombreDeRDVParMedecinAujourdHui = int.Parse(await nombreDeRDVParMedecinAujourdHuiResponse.Content.ReadAsStringAsync());

                var nombreDePendingRDVParMedecinUrl = $"{_configuration["ApiUrls:NombreDePendingRDVParMedecin"]}/{medecinId}";
                var nombreDePendingRDVParMedecinResponse = await _httpClient.GetAsync(nombreDePendingRDVParMedecinUrl);
                nombreDePendingRDVParMedecinResponse.EnsureSuccessStatusCode();
                stats.NombreDePendingAppointmentsByDoctor = int.Parse(await nombreDePendingRDVParMedecinResponse.Content.ReadAsStringAsync());
            }

            if (cliniqueId.HasValue)
            {
                var nombreDeRDVParCliniqueAujourdHuiUrl = $"{_configuration["ApiUrls:NombreDeRDVParClinique"]}/{cliniqueId}?date={DateTime.UtcNow:yyyy-MM-dd}";
                var nombreDeRDVParCliniqueAujourdHuiResponse = await _httpClient.GetAsync(nombreDeRDVParCliniqueAujourdHuiUrl);
                nombreDeRDVParCliniqueAujourdHuiResponse.EnsureSuccessStatusCode();
                stats.NombreDeRDVParCliniqueAujourdHui = int.Parse(await nombreDeRDVParCliniqueAujourdHuiResponse.Content.ReadAsStringAsync());

                var nombreDePendingRDVParCliniqueUrl = $"{_configuration["ApiUrls:NombreDePendingRDVParClinique"]}/{cliniqueId}";
                var nombreDePendingRDVParCliniqueResponse = await _httpClient.GetAsync(nombreDePendingRDVParCliniqueUrl);
                nombreDePendingRDVParCliniqueResponse.EnsureSuccessStatusCode();
                stats.NombreDePendingAppointmentsByClinic = int.Parse(await nombreDePendingRDVParCliniqueResponse.Content.ReadAsStringAsync());
            }

            if (cliniqueId.HasValue)
            {
                var revenuesMensuelsByClinicUrl = $"{_configuration["ApiUrls:RevenusMensuelsParClinique"]}/{cliniqueId}";
                var revenuesMensuelsByClinicResponse = await _httpClient.GetAsync(revenuesMensuelsByClinicUrl);
                revenuesMensuelsByClinicResponse.EnsureSuccessStatusCode();
                stats.RevenuesMensuelsByClinic = decimal.Parse(await revenuesMensuelsByClinicResponse.Content.ReadAsStringAsync());

                var revenuesMensuelsTrendByClinicUrl = $"{_configuration["ApiUrls:TrendRevenusMensuelsParClinique"]}/{cliniqueId}";
                var revenuesMensuelsTrendByClinicResponse = await _httpClient.GetAsync(revenuesMensuelsTrendByClinicUrl);
                revenuesMensuelsTrendByClinicResponse.EnsureSuccessStatusCode();
                var revenuesMensuelsTrendContent = await revenuesMensuelsTrendByClinicResponse.Content.ReadAsStringAsync();
                stats.RevenuesMensuelsByClinicTrend = JsonConvert.DeserializeObject<RevenusMensuelTrend>(revenuesMensuelsTrendContent);
            }

            if (!patientId.HasValue && !medecinId.HasValue && !cliniqueId.HasValue)
            {
                // Récupérer le trend des nouvelles cliniques
                var newClinicsTrend = await CalculerNewClinicsTrend(dateDebut, dateFin);
                stats.TrendDeNouvellesCliniques.Value = newClinicsTrend.Value;
                stats.TrendDeNouvellesCliniques.IsPositive = newClinicsTrend.IsPositive;
                // Récupérer le trend des nouveaux médecins
                var newDoctorsTrend = await CalculerNewDoctorsTrend(dateDebut, dateFin);
                stats.TrendDeNouveauxMedecins.Value = newDoctorsTrend.Value;
                stats.TrendDeNouveauxMedecins.IsPositive = newDoctorsTrend.IsPositive;
                // Récupérer le trend des nouveaux patients
                var newPatientsTrend = await CalculerNewPatientsTrend(dateDebut, dateFin);
                stats.TrendDeNouveauxPatients.Value = newPatientsTrend.Value;
                stats.TrendDeNouveauxPatients.IsPositive = newPatientsTrend.IsPositive;
            }

            return stats;
        }

    }
}

