using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Reporting.Application.DTOs;
using Reporting.Application.Services;
using Reporting.Domain.Interfaces;
using Reporting.Domain.ValueObject;
using Reporting.Infrastructure.Repositories;

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

            // Setup cache miss
            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);

            // Setup repo and mapper
            _repoMock.Setup(r => r.GetDashboardStatsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                     .ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<DashboardStatsDTO>(stats)).Returns(mapped);

            // Mock ICacheEntry
            var cacheEntryMock = new Mock<ICacheEntry>();
            _cacheMock.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

            var result = await _service.GetDashboardStatsAsync(DateTime.Today, DateTime.Today);

            Assert.Equal(mapped, result);
            _repoMock.Verify(r => r.GetDashboardStatsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
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
            var stats = new StatistiqueClinique { CliniqueId = id, Nom = "Test", NombreMedecins = 1 };
            var mapped = new ComparaisonCliniqueDTO { CliniqueId = id, Nom = "Test", NombreMedecins = 1 };

            _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);
            _repoMock.Setup(r => r.GetStatistiquesCliniqueAsync(id)).ReturnsAsync(stats);
            _mapperMock.Setup(m => m.Map<ComparaisonCliniqueDTO>(stats)).Returns(mapped);

            var cacheEntryMock = new Mock<ICacheEntry>();
            _cacheMock.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

            var result = await _service.ComparerCliniquesAsync(new List<Guid> { id });

            Assert.Single(result);
            Assert.Equal(id, result[0].CliniqueId);
        }
    }
}
