using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Reporting.Application.DTOs;
using Reporting.Application.Services;
using Reporting.Domain.Interfaces;
using Reporting.Domain.ValueObject;
using Reporting.Infrastructure.Repositories;
using Xunit;

namespace Reporting.Tests
{
    public class ReportingServiceTests
    {
        private readonly Mock<IReportingRepository> _repoMock;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ReportingService>> _loggerMock;
        private readonly ReportingService _service;

        public ReportingServiceTests()
        {
            _repoMock = new Mock<IReportingRepository>();
            _cacheMock = new Mock<IMemoryCache>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ReportingService>>();
            _service = new ReportingService(_repoMock.Object, _cacheMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetNombreConsultationsAsync_ReturnsFromCache()
        {
            int expected = 42;
            object cacheValue = expected;
            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);

            var result = await _service.GetNombreConsultationsAsync(DateTime.Today, DateTime.Today);

            Assert.Equal(expected, result);
            _repoMock.Verify(r => r.GetNombreConsultationsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task GetNombreConsultationsAsync_CacheMiss_CallsRepositoryAndSetsCache()
        {
            object cacheValue;
            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);
            _repoMock.Setup(r => r.GetNombreConsultationsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(7);

            var cacheEntryMock = new Mock<ICacheEntry>();
            _cacheMock.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

            var result = await _service.GetNombreConsultationsAsync(DateTime.Today, DateTime.Today);

            Assert.Equal(7, result);
            _repoMock.Verify(r => r.GetNombreConsultationsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task GetStatistiquesRendezVousAsync_ThrowsIfDateDebutAfterDateFin()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetStatistiquesRendezVousAsync(DateTime.Today.AddDays(1), DateTime.Today));
        }

        [Fact]
        public async Task GetStatistiquesRendezVousAsync_ReturnsFromCache()
        {
            var expected = new List<RendezVousStatDTO> { new RendezVousStatDTO() };
            object cacheValue = expected;
            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);

            var result = await _service.GetStatistiquesRendezVousAsync(DateTime.Today, DateTime.Today);

            Assert.Equal(expected, result);
            _repoMock.Verify(r => r.GetStatistiquesRendezVousAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task GetStatistiquesRendezVousAsync_CacheMiss_MapsAndSetsCache()
        {
            object cacheValue;
            var stats = new List<RendezVousStat> { new RendezVousStat { Date = DateTime.Today } };
            var mapped = new List<RendezVousStatDTO> { new RendezVousStatDTO { Date = DateTime.Today } };

            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);
            _repoMock.Setup(r => r.GetStatistiquesRendezVousAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<List<RendezVousStatDTO>>(stats)).Returns(mapped);

            var cacheEntryMock = new Mock<ICacheEntry>();
            _cacheMock.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

            var result = await _service.GetStatistiquesRendezVousAsync(DateTime.Today, DateTime.Today);

            Assert.Equal(mapped, result);
            _repoMock.Verify(r => r.GetStatistiquesRendezVousAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task GetStatistiquesRendezVousAsync_ThrowsIfNoStats()
        {
            object cacheValue;
            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);
            _repoMock.Setup(r => r.GetStatistiquesRendezVousAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(new List<RendezVousStat>());

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetStatistiquesRendezVousAsync(DateTime.Today, DateTime.Today));
        }

        [Fact]
        public async Task GetNombreNouveauxPatientsAsync_CallsRepository()
        {
            _repoMock.Setup(r => r.GetNombreNouveauxPatientsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(5);

            var result = await _service.GetNombreNouveauxPatientsAsync(DateTime.Today, DateTime.Today);

            Assert.Equal(5, result);
        }

        [Fact]
        public async Task GetNombreMedecinParSpecialite_MapsResult()
        {
            var stats = new List<DoctorStats> { new DoctorStats { Cle = "A", Nombre = 1 } };
            var mapped = new List<DoctorStatsDTO> { new DoctorStatsDTO { Cle = "A", Nombre = 1 } };
            _repoMock.Setup(r => r.GetNombreMedecinParSpecialiteAsync()).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<List<DoctorStatsDTO>>(stats)).Returns(mapped);

            var result = await _service.GetNombreMedecinParSpecialite();

            Assert.Equal(mapped, result);
        }

        [Fact]
        public async Task GetNombreMedecinByClinique_MapsResult()
        {
            var stats = new List<DoctorStats> { new DoctorStats { Cle = "B", Nombre = 2 } };
            var mapped = new List<DoctorStatsDTO> { new DoctorStatsDTO { Cle = "B", Nombre = 2 } };
            _repoMock.Setup(r => r.GetNombreMedecinByCliniqueAsync()).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<List<DoctorStatsDTO>>(stats)).Returns(mapped);

            var result = await _service.GetNombreMedecinByClinique();

            Assert.Equal(mapped, result);
        }

        [Fact]
        public async Task GetNombreMedecinBySpecialiteDansUneClinique_MapsResult()
        {
            var stats = new List<DoctorStats> { new DoctorStats { Cle = "C", Nombre = 3 } };
            var mapped = new List<DoctorStatsDTO> { new DoctorStatsDTO { Cle = "C", Nombre = 3 } };
            var cliniqueId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetNombreMedecinBySpecialiteDansUneCliniqueAsync(cliniqueId)).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<List<DoctorStatsDTO>>(stats)).Returns(mapped);

            var result = await _service.GetNombreMedecinBySpecialiteDansUneClinique(cliniqueId);

            Assert.Equal(mapped, result);
        }

        [Fact]
        public async Task GetNombreDeFactureByStatus_MapsResult()
        {
            var stats = new List<FactureStats> { new FactureStats { Cle = "Payée", Nombre = 10 } };
            var mapped = new List<FactureStatsDTO> { new FactureStatsDTO { Cle = "Payée", Nombre = 10 } };
            _repoMock.Setup(r => r.GetNombreDeFactureByStatusAsync()).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<List<FactureStatsDTO>>(stats)).Returns(mapped);

            var result = await _service.GetNombreDeFactureByStatus();

            Assert.Equal(mapped, result);
        }

        [Fact]
        public async Task GetNombreDeFactureParClinique_MapsResult()
        {
            var stats = new List<FactureStats> { new FactureStats { Cle = "CliniqueA", Nombre = 5 } };
            var mapped = new List<FactureStatsDTO> { new FactureStatsDTO { Cle = "CliniqueA", Nombre = 5 } };
            _repoMock.Setup(r => r.GetNombreDeFactureParCliniqueAsync()).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<List<FactureStatsDTO>>(stats)).Returns(mapped);

            var result = await _service.GetNombreDeFactureParClinique();

            Assert.Equal(mapped, result);
        }

        [Fact]
        public async Task GetNombreDeFactureParStatusParClinique_MapsResult()
        {
            var stats = new List<FactureStats> { new FactureStats { Cle = "Payée", Nombre = 7 } };
            var mapped = new List<FactureStatsDTO> { new FactureStatsDTO { Cle = "Payée", Nombre = 7 } };
            _repoMock.Setup(r => r.GetNombreDeFacturesByStatusParCliniqueAsync()).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<List<FactureStatsDTO>>(stats)).Returns(mapped);

            var result = await _service.GetNombreDeFactureParStatusParClinique();

            Assert.Equal(mapped, result);
        }

        [Fact]
        public async Task GetNombreDeFacturesByStatusDansUneClinique_MapsResult()
        {
            var stats = new List<FactureStats> { new FactureStats { Cle = "Impayée", Nombre = 2 } };
            var mapped = new List<FactureStatsDTO> { new FactureStatsDTO { Cle = "Impayée", Nombre = 2 } };
            var cliniqueId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetNombreDeFacturesByStatusDansUneCliniqueAsync(cliniqueId)).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<List<FactureStatsDTO>>(stats)).Returns(mapped);

            var result = await _service.GetNombreDeFacturesByStatusDansUneClinique(cliniqueId);

            Assert.Equal(mapped, result);
        }

        [Fact]
        public async Task GetNombreDeCliniques_CallsRepository()
        {
            _repoMock.Setup(r => r.GetNombreDeCliniquesAsync()).ReturnsAsync(4);

            var result = await _service.GetNombreDeCliniques();

            Assert.Equal(4, result);
        }

        [Fact]
        public async Task GetNombreNouvellesCliniquesDuMois_CallsRepository()
        {
            _repoMock.Setup(r => r.GetNombreNouvellesCliniquesDuMoisAsync()).ReturnsAsync(2);

            var result = await _service.GetNombreNouvellesCliniquesDuMois();

            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetNombreNouvellesCliniquesParMois_MapsResult()
        {
            var stats = new List<Statistique> { new Statistique { Cle = "202406", Nombre = 1 } };
            var mapped = new List<StatistiqueDTO> { new StatistiqueDTO { Cle = "202406", Nombre = 1 } };
            _repoMock.Setup(r => r.GetNombreNouvellesCliniquesParMoisAsync()).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<List<StatistiqueDTO>>(stats)).Returns(mapped);

            var result = await _service.GetNombreNouvellesCliniquesParMois();

            Assert.Equal(mapped, result);
        }

        [Fact]
        public async Task GetStatistiquesClinique_MapsResult()
        {
            var stats = new StatistiqueClinique { CliniqueId = Guid.NewGuid(), Nom = "CliniqueX", NombreMedecins = 3, NombrePatients = 10, NombreConsultations = 20, NombreRendezVous = 5 };
            var mapped = new StatistiqueCliniqueDTO { CliniqueId = stats.CliniqueId, Nom = "CliniqueX", NombreMedecins = 3, NombrePatients = 10, NombreConsultations = 20, NombreRendezVous = 5 };
            _repoMock.Setup(r => r.GetStatistiquesCliniqueAsync(stats.CliniqueId)).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<StatistiqueCliniqueDTO>(stats)).Returns(mapped);

            var result = await _service.GetStatistiquesClinique(stats.CliniqueId);

            Assert.Equal(mapped, result);
        }

        [Fact]
        public async Task GetActivitesMedecin_MapsResult()
        {
            var activites = new List<ActiviteMedecin> { new ActiviteMedecin { MedecinId = Guid.NewGuid(), NomComplet = "Dr X", NombreConsultations = 5, NombreRendezVous = 10 } };
            var mapped = new List<ActiviteMedecinDTO> { new ActiviteMedecinDTO { MedecinId = activites[0].MedecinId, NomComplet = "Dr X", NombreConsultations = 5, NombreRendezVous = 10 } };
            _repoMock.Setup(r => r.GetActivitesMedecinAsync(activites[0].MedecinId)).ReturnsAsync(activites);
            _mapperMock.Setup(m => m.Map<List<ActiviteMedecinDTO>>(activites)).Returns(mapped);

            var result = await _service.GetActivitesMedecin(activites[0].MedecinId);

            Assert.Equal(mapped, result);
        }

        [Fact]
        public async Task GetMontantPaiementsAsync_CallsRepository()
        {
            _repoMock.Setup(r => r.GetMontantPaiementsAsync("payé", It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(100.5m);

            var result = await _service.GetMontantPaiementsAsync("payé", DateTime.Today, DateTime.Today);

            Assert.Equal(100.5m, result);
        }

        [Fact]
        public async Task GetStatistiquesFacturesAsync_MapsResult()
        {
            var stats = new StatistiquesFacture { NombreTotal = 10, NombrePayees = 5, NombreImpayees = 3, NombrePartiellementPayees = 2, MontantTotal = 1000, MontantTotalPaye = 500, NombreParClinique = new Dictionary<Guid, int>() };
            var mapped = new StatistiquesFactureDto { NombreTotal = 10, NombrePayees = 5, NombreImpayees = 3, NombrePartiellementPayees = 2, MontantTotal = 1000, MontantTotalPaye = 500, NombreParClinique = new Dictionary<Guid, int>() };
            _repoMock.Setup(r => r.GetStatistiquesFacturesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<StatistiquesFactureDto>(stats)).Returns(mapped);

            var result = await _service.GetStatistiquesFacturesAsync(DateTime.Today, DateTime.Today);

            Assert.Equal(mapped, result);
        }

        [Fact]
        public async Task ComparerCliniquesAsync_ReturnsEmptyIfNullOrEmpty()
        {
            var result1 = await _service.ComparerCliniquesAsync(null);
            var result2 = await _service.ComparerCliniquesAsync(new List<Guid>());

            Assert.Empty(result1);
            Assert.Empty(result2);
        }

        [Fact]
        public async Task ComparerCliniquesAsync_ReturnsFromCache()
        {
            var expected = new List<ComparaisonCliniqueDTO> { new ComparaisonCliniqueDTO() };
            object cacheValue = expected;
            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);

            var result = await _service.ComparerCliniquesAsync(new List<Guid> { Guid.NewGuid() });

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ComparerCliniquesAsync_CacheMiss_CallsRepositoryAndSetsCache()
        {
            object cacheValue;
            var id = Guid.NewGuid();
            var stats = new StatistiqueClinique { CliniqueId = id, Nom = "Test", NombreMedecins = 1, NombrePatients = 2, NombreConsultations = 3, NombreRendezVous = 4 };
            var mapped = new ComparaisonCliniqueDTO { CliniqueId = id, Nom = "Test", NombreMedecins = 1, NombrePatients = 2, NombreConsultations = 3, NombreRendezVous = 4 };

            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);
            _repoMock.Setup(r => r.GetStatistiquesCliniqueAsync(id)).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<ComparaisonCliniqueDTO>(stats)).Returns(mapped);

            var cacheEntryMock = new Mock<ICacheEntry>();
            _cacheMock.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

            var result = await _service.ComparerCliniquesAsync(new List<Guid> { id });

            Assert.Single(result);
            Assert.Equal(id, result[0].CliniqueId);
        }

        [Fact]
        public async Task GetDashboardStatsAsync_ReturnsFromCache()
        {
            var expected = new DashboardStatsDTO { ConsultationsJour = 1 };
            object cacheValue = expected;
            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);

            var result = await _service.GetDashboardStatsAsync(DateTime.Today, DateTime.Today);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetDashboardStatsAsync_CacheMiss_MapsAndSetsCache()
        {
            object cacheValue;
            var stats = new DashboardStats { ConsultationsJour = 2 };
            var mapped = new DashboardStatsDTO { ConsultationsJour = 2 };

            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);
            _repoMock.Setup(r => r.GetDashboardStatsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, null, null)).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<DashboardStatsDTO>(stats)).Returns(mapped);

            var cacheEntryMock = new Mock<ICacheEntry>();
            _cacheMock.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

            var result = await _service.GetDashboardStatsAsync(DateTime.Today, DateTime.Today);

            Assert.Equal(mapped, result);
            _repoMock.Verify(r => r.GetDashboardStatsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, null, null), Times.Once);
        }
    }
}
