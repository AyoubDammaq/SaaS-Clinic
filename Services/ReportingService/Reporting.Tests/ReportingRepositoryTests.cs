using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Reporting.Domain.ValueObject;
using Reporting.Infrastructure.Repositories;
using System.Net;
using Xunit;

namespace Reporting.Tests
{
    public class ReportingRepositoryTests
    {
        private HttpClient CreateHttpClient(HttpStatusCode statusCode, string content)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                });
            return new HttpClient(handlerMock.Object);
        }

        private IConfiguration CreateConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"ApiUrls:Consultation", "http://fake/consultation"},
                {"ApiUrls:RendezVous", "http://fake/rendezvous"},
                {"ApiUrls:Patients", "http://fake/patients"},
                {"ApiUrls:MedecinSpecialite", "http://fake/medecin/specialite"},
                {"ApiUrls:MedecinClinique", "http://fake/medecin/clinique"},
                {"ApiUrls:MedecinSpecialiteClinique", "http://fake/medecin/specialite/clinique"},
                {"ApiUrls:FactureStatus", "http://fake/facture/status"},
                {"ApiUrls:FactureClinic", "http://fake/facture/clinic"},
                {"ApiUrls:FactureStatusClinic", "http://fake/facture/status/clinic"},
                {"ApiUrls:CliniqueNombre", "http://fake/clinique/nombre"},
                {"ApiUrls:CliniqueNouvellesMois", "http://fake/clinique/nouvelles-mois"},
                {"ApiUrls:CliniqueNouvellesParMois", "http://fake/clinique/nouvelles-par-mois"},
                {"ApiUrls:CliniqueStats", "http://fake/clinique/stats"},
                {"ApiUrls:MedecinActivites", "http://fake/medecin/activites"},
                {"ApiUrls:PaiementMontant", "http://fake/paiement/montant"},
                {"ApiUrls:FactureCount", "http://fake/facture/count"},
                {"ApiUrls:FactureMontant", "http://fake/facture/montant"},
                {"ApiUrls:FactureStatistiques", "http://fake/facture/statistiques"},
                {"ApiUrls:RendezVousHebdomadaireByDoctor", "http://fake/rdv/doctor"},
                {"ApiUrls:RendezVousHebdomadaireByClinic", "http://fake/rdv/clinic"}
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task GetNombreConsultationsAsync_ReturnsInt()
        {
            var httpClient = CreateHttpClient(HttpStatusCode.OK, "5");
            var config = CreateConfiguration();
            var repo = new ReportingRepository(httpClient, config);

            var result = await repo.GetNombreConsultationsAsync(DateTime.Now, DateTime.Now);

            Assert.Equal(5, result);
        }

        [Fact]
        public async Task GetNombreConsultationsAsync_ThrowsOnError()
        {
            var httpClient = CreateHttpClient(HttpStatusCode.BadRequest, "");
            var config = CreateConfiguration();
            var repo = new ReportingRepository(httpClient, config);

            await Assert.ThrowsAsync<Exception>(() =>
                repo.GetNombreConsultationsAsync(DateTime.Now, DateTime.Now));
        }

        [Fact]
        public async Task GetStatistiquesRendezVousAsync_ReturnsList()
        {
            var stats = new List<RendezVousStat>
            {
                new RendezVousStat { Date = DateTime.Today, TotalRendezVous = 2, Confirmes = 1, Annules = 0, EnAttente = 1 }
            };
            var httpClient = CreateHttpClient(HttpStatusCode.OK, JsonConvert.SerializeObject(stats));
            var config = CreateConfiguration();
            var repo = new ReportingRepository(httpClient, config);

            var result = await repo.GetStatistiquesRendezVousAsync(DateTime.Now, DateTime.Now);

            Assert.Single(result);
            Assert.Equal(2, result.First().TotalRendezVous);
        }

        [Fact]
        public async Task GetStatistiquesRendezVousAsync_ThrowsOnEmptyList()
        {
            var httpClient = CreateHttpClient(HttpStatusCode.OK, JsonConvert.SerializeObject(new List<RendezVousStat>()));
            var config = CreateConfiguration();
            var repo = new ReportingRepository(httpClient, config);

            await Assert.ThrowsAsync<Exception>(() =>
                repo.GetStatistiquesRendezVousAsync(DateTime.Now, DateTime.Now));
        }

        [Fact]
        public async Task GetStatistiquesRendezVousAsync_ThrowsOnError()
        {
            var httpClient = CreateHttpClient(HttpStatusCode.BadRequest, "");
            var config = CreateConfiguration();
            var repo = new ReportingRepository(httpClient, config);

            await Assert.ThrowsAsync<Exception>(() =>
                repo.GetStatistiquesRendezVousAsync(DateTime.Now, DateTime.Now));
        }

        [Fact]
        public async Task GetNombreNouveauxPatientsAsync_ReturnsInt()
        {
            var httpClient = CreateHttpClient(HttpStatusCode.OK, "3");
            var config = CreateConfiguration();
            var repo = new ReportingRepository(httpClient, config);

            var result = await repo.GetNombreNouveauxPatientsAsync(DateTime.Now, DateTime.Now);

            Assert.Equal(3, result);
        }

        [Fact]
        public async Task GetNombreMedecinParSpecialiteAsync_ReturnsList()
        {
            var doctors = new List<DoctorStats>
            {
                new DoctorStats { Cle = "Cardio", Nombre = 2 }
            };
            var httpClient = CreateHttpClient(HttpStatusCode.OK, JsonConvert.SerializeObject(doctors));
            var config = CreateConfiguration();
            var repo = new ReportingRepository(httpClient, config);

            var result = await repo.GetNombreMedecinParSpecialiteAsync();

            Assert.Single(result);
            Assert.Equal("Cardio", result[0].Cle);
        }

        [Fact]
        public async Task GetDashboardStatsAsync_ReturnsDashboardStats()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage req, CancellationToken token) =>
                {
                    if (req.RequestUri != null && req.RequestUri.ToString().Contains("montant"))
                        return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("2.5") };
                    if (req.RequestUri != null && req.RequestUri.ToString().Contains("statistiques"))
                    {
                        var statsFacture = new StatistiquesFacture
                        {
                            NombreTotal = 1,
                            NombrePayees = 1,
                            NombreImpayees = 1,
                            NombrePartiellementPayees = 1,
                            MontantTotal = 2.5m,
                            MontantTotalPaye = 2.5m,
                            NombreParClinique = new Dictionary<Guid, int>()
                        };
                        return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(statsFacture)) };
                    }
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("1") };
                });
            var httpClient = new HttpClient(handlerMock.Object);
            var config = CreateConfiguration();
            var repo = new ReportingRepository(httpClient, config);

            var stats = await repo.GetDashboardStatsAsync(DateTime.Now, DateTime.Now);

            Assert.Equal(1, stats.ConsultationsJour);
            Assert.Equal(1, stats.NouveauxPatients);
            Assert.Equal(1, stats.NombreFactures);
            Assert.Equal(2.5m, stats.TotalFacturesPayees);
            Assert.Equal(2.5m, stats.TotalFacturesImpayees);
            Assert.Equal(2.5m, stats.PaiementsPayes);
            Assert.Equal(2.5m, stats.PaiementsImpayes);
            Assert.Equal(1, stats.PaiementsEnAttente);
        }
    }
}

