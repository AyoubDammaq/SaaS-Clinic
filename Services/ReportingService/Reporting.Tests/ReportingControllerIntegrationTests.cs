using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Reporting.Tests
{
    public class ReportingControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ReportingControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", GenerateJwtToken());
        }

        private static string GenerateJwtToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ByYM000OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SNM"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "AuthService",
                audience: "AuthService",
                claims: new[] { new Claim(ClaimTypes.Name, "test-user") },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string FormatDate(DateTime date) => date.ToString("yyyy-MM-dd");

        [Fact]
        public async Task GetDashboardStats_ReturnsSuccess()
        {
            var url = $"/api/Reporting/dashboard?start={FormatDate(DateTime.Today.AddDays(-1))}&end={FormatDate(DateTime.Today)}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ExportDashboardExcel_ReturnsSuccess()
        {
            var url = $"/api/Reporting/dashboard/excel?start={FormatDate(DateTime.Today.AddDays(-1))}&end={FormatDate(DateTime.Today)}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task ExportDashboardPdf_ReturnsSuccess()
        {
            var url = $"/api/Reporting/dashboard/pdf?start={FormatDate(DateTime.Today.AddDays(-1))}&end={FormatDate(DateTime.Today)}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task GetNombreCliniques_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/api/Reporting/cliniques/count");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNombreNouveauxCliniques_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/api/Reporting/cliniques/nouveaux/count");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNombreNouveauxCliniquesParMois_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/api/Reporting/cliniques/nouveaux/mois");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNombreConsultations_ReturnsSuccess()
        {
            var url = $"/api/Reporting/consultations/count?start={FormatDate(DateTime.Today.AddDays(-1))}&end={FormatDate(DateTime.Today)}";
            var response = await _client.GetAsync(url);
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetStatistiquesRendezVous_ReturnsSuccess()
        {
            var url = $"/api/Reporting/rendezvous/stats?start={FormatDate(DateTime.Today.AddDays(-1))}&end={FormatDate(DateTime.Today)}";
            var response = await _client.GetAsync(url);
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetNombreNouveauxPatients_ReturnsSuccess()
        {
            var url = $"/api/Reporting/patients/nouveaux/count?start={FormatDate(DateTime.Today.AddDays(-1))}&end={FormatDate(DateTime.Today)}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNombreMedecinParSpecialite_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/api/Reporting/medecins/specialites/count");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNombreMedecinByClinique_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/api/Reporting/medecins/cliniques/count");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNombreMedecinBySpecialiteDansUneClinique_ReturnsSuccess()
        {
            var id = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/Reporting/medecins/specialites/cliniques/{id}/count");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNombreDeFactureByStatus_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/api/Reporting/factures/status/count");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNombreDeFactureParClinique_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/api/Reporting/factures/cliniques/count");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNombreDeFactureParStatusParClinique_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/api/Reporting/factures/status/cliniques/count");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNombreDeFacturesByStatusDansUneClinique_ReturnsSuccess()
        {
            var id = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/Reporting/factures/status/cliniques/{id}/count");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetStatistiquesClinique_ReturnsSuccess()
        {
            var id = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/Reporting/cliniques/{id}/stats");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetActivitesMedecin_ReturnsSuccess()
        {
            var id = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/Reporting/medecins/{id}/activites");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMontantPaiements_ReturnsSuccess()
        {
            var url = $"/api/Reporting/paiements/montant?statut=payé&start={FormatDate(DateTime.Today.AddDays(-1))}&end={FormatDate(DateTime.Today)}";
            var response = await _client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ComparerCliniques_ReturnsBadRequest_WhenListIsEmpty()
        {
            var content = new StringContent("[]", Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Reporting/cliniques/comparaison", content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

