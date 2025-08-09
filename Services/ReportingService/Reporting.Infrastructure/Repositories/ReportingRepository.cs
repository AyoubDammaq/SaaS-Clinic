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

        public async Task<DashboardStats> GetDashboardStatsAsync(DateTime dateDebut, DateTime dateFin, Guid? patientId = null, Guid? medecinId = null, Guid? cliniqueId = null)
        {
            var stats = new DashboardStats();

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

            // Récupérer les statistiques de rendez-vous
            var rdvUrl = _configuration["ApiUrls:RendezVous"];
            if (medecinId.HasValue)
                rdvUrl += $"/doctor/{medecinId}";
            else if (cliniqueId.HasValue)
                rdvUrl += $"/clinic/{cliniqueId}";
            rdvUrl += $"?start={dateDebut:yyyy-MM-ddTHH:mm:ss}&end={dateFin:yyyy-MM-ddTHH:mm:ss}";
            var rdvResponse = await _httpClient.GetAsync(rdvUrl);
            if (rdvResponse.IsSuccessStatusCode)
            {
                var rdvContent = await rdvResponse.Content.ReadAsStringAsync();
                stats.RendezvousStats = JsonConvert.DeserializeObject<List<RendezVousStat>>(rdvContent);
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

            return stats;
        }
    }
}

