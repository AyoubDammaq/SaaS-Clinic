using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Reporting.API.Controllers;
using Reporting.Application.DTOs;
using Reporting.Application.Interfaces;
using Xunit;

namespace Reporting.Tests
{
    public class ReportingControllerTests
    {
        private readonly Mock<IReportingService> _serviceMock;
        private readonly Mock<ILogger<ReportingController>> _loggerMock;
        private readonly ReportingController _controller;

        public ReportingControllerTests()
        {
            _serviceMock = new Mock<IReportingService>();
            _loggerMock = new Mock<ILogger<ReportingController>>();
            _controller = new ReportingController(_serviceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetNombreConsultations_ReturnsOk_WhenResultFound()
        {
            _serviceMock.Setup(s => s.GetNombreConsultationsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(5);

            var result = await _controller.GetNombreConsultations(DateTime.Today, DateTime.Today);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(5, ok.Value);
        }

        [Fact]
        public async Task GetNombreConsultations_ReturnsNotFound_WhenResultIsZero()
        {
            _serviceMock.Setup(s => s.GetNombreConsultationsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(0);

            var result = await _controller.GetNombreConsultations(DateTime.Today, DateTime.Today);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Aucune statistique", notFound.Value?.ToString());
        }

        [Fact]
        public async Task GetNombreConsultations_Returns500_OnException()
        {
            _serviceMock.Setup(s => s.GetNombreConsultationsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ThrowsAsync(new Exception());

            var result = await _controller.GetNombreConsultations(DateTime.Today, DateTime.Today);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task GetStatistiquesRendezVous_ReturnsOk_WhenResultFound()
        {
            var list = new List<RendezVousStatDTO> { new RendezVousStatDTO() };
            _serviceMock.Setup(s => s.GetStatistiquesRendezVousAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(list);

            var result = await _controller.GetStatistiquesRendezVous(DateTime.Today, DateTime.Today);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetStatistiquesRendezVous_ReturnsNotFound_WhenEmpty()
        {
            _serviceMock.Setup(s => s.GetStatistiquesRendezVousAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(new List<RendezVousStatDTO>());

            var result = await _controller.GetStatistiquesRendezVous(DateTime.Today, DateTime.Today);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Aucune statistique", notFound.Value?.ToString());
        }

        [Fact]
        public async Task GetStatistiquesRendezVous_Returns500_OnException()
        {
            _serviceMock.Setup(s => s.GetStatistiquesRendezVousAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ThrowsAsync(new Exception());

            var result = await _controller.GetStatistiquesRendezVous(DateTime.Today, DateTime.Today);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task GetNombreNouveauxPatients_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetNombreNouveauxPatientsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(2);

            var result = await _controller.GetNombreNouveauxPatients(DateTime.Today, DateTime.Today);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(2, ok.Value);
        }

        [Fact]
        public async Task GetNombreNouveauxPatients_Returns500_OnException()
        {
            _serviceMock.Setup(s => s.GetNombreNouveauxPatientsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ThrowsAsync(new Exception());

            var result = await _controller.GetNombreNouveauxPatients(DateTime.Today, DateTime.Today);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task GetNombreMedecinParSpecialite_ReturnsOk()
        {
            var list = new List<DoctorStatsDTO> { new DoctorStatsDTO() };
            _serviceMock.Setup(s => s.GetNombreMedecinParSpecialite()).ReturnsAsync(list);

            var result = await _controller.GetNombreMedecinParSpecialite();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetNombreMedecinParSpecialite_ReturnsNotFound_WhenEmpty()
        {
            _serviceMock.Setup(s => s.GetNombreMedecinParSpecialite()).ReturnsAsync(new List<DoctorStatsDTO>());

            var result = await _controller.GetNombreMedecinParSpecialite();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Aucune statistique", notFound.Value?.ToString());
        }

        [Fact]
        public async Task GetNombreMedecinParSpecialite_Returns500_OnException()
        {
            _serviceMock.Setup(s => s.GetNombreMedecinParSpecialite()).ThrowsAsync(new Exception());

            var result = await _controller.GetNombreMedecinParSpecialite();

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task GetNombreMedecinByClinique_ReturnsOk()
        {
            var list = new List<DoctorStatsDTO> { new DoctorStatsDTO() };
            _serviceMock.Setup(s => s.GetNombreMedecinByClinique()).ReturnsAsync(list);

            var result = await _controller.GetNombreMedecinByClinique();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetNombreMedecinBySpecialiteDansUneClinique_ReturnsOk()
        {
            var list = new List<DoctorStatsDTO> { new DoctorStatsDTO() };
            _serviceMock.Setup(s => s.GetNombreMedecinBySpecialiteDansUneClinique(It.IsAny<Guid>())).ReturnsAsync(list);

            var result = await _controller.GetNombreMedecinBySpecialiteDansUneClinique(Guid.NewGuid());

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetNombreDeFactureByStatus_ReturnsOk()
        {
            var list = new List<FactureStatsDTO> { new FactureStatsDTO() };
            _serviceMock.Setup(s => s.GetNombreDeFactureByStatus()).ReturnsAsync(list);

            var result = await _controller.GetNombreDeFactureByStatus();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetNombreDeFactureParClinique_ReturnsOk()
        {
            var list = new List<FactureStatsDTO> { new FactureStatsDTO() };
            _serviceMock.Setup(s => s.GetNombreDeFactureParClinique()).ReturnsAsync(list);

            var result = await _controller.GetNombreDeFactureParClinique();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetNombreDeFactureParStatusParClinique_ReturnsOk()
        {
            var list = new List<FactureStatsDTO> { new FactureStatsDTO() };
            _serviceMock.Setup(s => s.GetNombreDeFactureParStatusParClinique()).ReturnsAsync(list);

            var result = await _controller.GetNombreDeFactureParStatusParClinique();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetNombreDeFacturesByStatusDansUneClinique_ReturnsOk()
        {
            var list = new List<FactureStatsDTO> { new FactureStatsDTO() };
            _serviceMock.Setup(s => s.GetNombreDeFacturesByStatusDansUneClinique(It.IsAny<Guid>())).ReturnsAsync(list);

            var result = await _controller.GetNombreDeFacturesByStatusDansUneClinique(Guid.NewGuid());

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetNombreCliniques_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetNombreDeCliniques()).ReturnsAsync(3);

            var result = await _controller.GetNombreCliniques();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(3, ok.Value);
        }

        [Fact]
        public async Task GetNombreNouveauxCliniques_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetNombreNouvellesCliniquesDuMois()).ReturnsAsync(1);

            var result = await _controller.GetNombreNouveauxCliniques();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, ok.Value);
        }

        [Fact]
        public async Task GetNombreNouveauxCliniquesParMois_ReturnsOk()
        {
            var list = new List<StatistiqueDTO> { new StatistiqueDTO() };
            _serviceMock.Setup(s => s.GetNombreNouvellesCliniquesParMois()).ReturnsAsync(list);

            var result = await _controller.GetNombreNouveauxCliniquesParMois();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetStatistiquesClinique_ReturnsOk()
        {
            var dto = new StatistiqueCliniqueDTO();
            _serviceMock.Setup(s => s.GetStatistiquesClinique(It.IsAny<Guid>())).ReturnsAsync(dto);

            var result = await _controller.GetStatistiquesClinique(Guid.NewGuid());

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task GetActivitesMedecin_ReturnsOk()
        {
            var list = new List<ActiviteMedecinDTO> { new ActiviteMedecinDTO() };
            _serviceMock.Setup(s => s.GetActivitesMedecin(It.IsAny<Guid>())).ReturnsAsync(list);

            var result = await _controller.GetActivitesMedecin(Guid.NewGuid());

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetMontantPaiements_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetMontantPaiementsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(10.5m);

            var result = await _controller.GetMontantPaiements("payee", DateTime.Today, DateTime.Today);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(10.5m, ok.Value);
        }

        [Fact]
        public async Task GetStatistiquesFactures_ReturnsOk()
        {
            var dto = new StatistiquesFactureDto();
            _serviceMock.Setup(s => s.GetStatistiquesFacturesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(dto);

            var result = await _controller.GetStatistiquesFactures(DateTime.Today, DateTime.Today);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task ComparerCliniques_ReturnsBadRequest_IfListNullOrEmpty()
        {
            var result1 = await _controller.ComparerCliniques(null);
            var result2 = await _controller.ComparerCliniques(new List<Guid>());

            Assert.IsType<BadRequestObjectResult>(result1);
            Assert.IsType<BadRequestObjectResult>(result2);
        }

        [Fact]
        public async Task ComparerCliniques_ReturnsOk()
        {
            var list = new List<ComparaisonCliniqueDTO> { new ComparaisonCliniqueDTO() };
            _serviceMock.Setup(s => s.ComparerCliniquesAsync(It.IsAny<List<Guid>>())).ReturnsAsync(list);

            var result = await _controller.ComparerCliniques(new List<Guid> { Guid.NewGuid() });

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetDashboardStats_ReturnsOk()
        {
            var dto = new DashboardStatsDTO();
            _serviceMock.Setup(s => s.GetDashboardStatsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, null, null)).ReturnsAsync(dto);

            var result = await _controller.GetDashboardStats(DateTime.Today, DateTime.Today);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task GetDashboardStats_Returns500_OnException()
        {
            _serviceMock.Setup(s => s.GetDashboardStatsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, null, null)).ThrowsAsync(new Exception());

            var result = await _controller.GetDashboardStats(DateTime.Today, DateTime.Today);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task GetStatistiquesHebdomadairesRendezVousByDoctor_ReturnsOk()
        {
            var list = new List<AppointmentDayStatDto> { new AppointmentDayStatDto() };
            _serviceMock.Setup(s => s.GetStatistiquesHebdomadairesRendezVousByDoctorAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(list);

            var result = await _controller.GetStatistiquesHebdomadairesRendezVousByDoctor(Guid.NewGuid(), DateTime.Today, DateTime.Today);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetStatistiquesHebdomadairesRendezVousByDoctor_Returns500_OnException()
        {
            _serviceMock.Setup(s => s.GetStatistiquesHebdomadairesRendezVousByDoctorAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ThrowsAsync(new Exception());

            var result = await _controller.GetStatistiquesHebdomadairesRendezVousByDoctor(Guid.NewGuid(), DateTime.Today, DateTime.Today);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task GetStatistiquesHebdomadairesRendezVousByClinic_ReturnsOk()
        {
            var list = new List<AppointmentDayStatDto> { new AppointmentDayStatDto() };
            _serviceMock.Setup(s => s.GetStatistiquesHebdomadairesRendezVousByClinicAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(list);

            var result = await _controller.GetStatistiquesHebdomadairesRendezVousByClinic(Guid.NewGuid(), DateTime.Today, DateTime.Today);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetStatistiquesHebdomadairesRendezVousByClinic_Returns500_OnException()
        {
            _serviceMock.Setup(s => s.GetStatistiquesHebdomadairesRendezVousByClinicAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ThrowsAsync(new Exception());

            var result = await _controller.GetStatistiquesHebdomadairesRendezVousByClinic(Guid.NewGuid(), DateTime.Today, DateTime.Today);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }
    }
}

