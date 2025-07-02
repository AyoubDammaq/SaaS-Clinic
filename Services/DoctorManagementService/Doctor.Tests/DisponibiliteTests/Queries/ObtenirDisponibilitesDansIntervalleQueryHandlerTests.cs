using Doctor.Application.AvailibilityServices.Queries.ObtenirDisponibilitesDansIntervalle;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Queries
{
    public class ObtenirDisponibilitesDansIntervalleQueryHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly ObtenirDisponibilitesDansIntervalleQueryHandler _handler;

        public ObtenirDisponibilitesDansIntervalleQueryHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new ObtenirDisponibilitesDansIntervalleQueryHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdIsEmpty()
        {
            // Arrange
            var query = new ObtenirDisponibilitesDansIntervalleQuery(Guid.Empty, DateTime.Now, DateTime.Now.AddHours(1));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenStartIsAfterOrEqualEnd()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var start = DateTime.Now.AddHours(2);
            var end = DateTime.Now;
            var query = new ObtenirDisponibilitesDansIntervalleQuery(medecinId, start, end);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnDisponibilites_WhenRepositoryReturnsList()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var start = new DateTime(2024, 6, 1, 8, 0, 0);
            var end = new DateTime(2024, 6, 1, 18, 0, 0);
            var disponibilites = new List<Disponibilite>
            {
                new Disponibilite
                {
                    Id = Guid.NewGuid(),
                    Jour = DayOfWeek.Monday,
                    HeureDebut = new TimeSpan(9, 0, 0),
                    HeureFin = new TimeSpan(12, 0, 0),
                    MedecinId = medecinId
                }
            };
            var query = new ObtenirDisponibilitesDansIntervalleQuery(medecinId, start, end);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibilitesDansIntervalleAsync(medecinId, start, end))
                .ReturnsAsync(disponibilites);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(disponibilites[0].Id, result[0].Id);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsNull()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var start = new DateTime(2024, 6, 1, 8, 0, 0);
            var end = new DateTime(2024, 6, 1, 18, 0, 0);
            var query = new ObtenirDisponibilitesDansIntervalleQuery(medecinId, start, end);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibilitesDansIntervalleAsync(medecinId, start, end))
                .ReturnsAsync((List<Disponibilite>)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsEmptyList()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var start = new DateTime(2024, 6, 1, 8, 0, 0);
            var end = new DateTime(2024, 6, 1, 18, 0, 0);
            var query = new ObtenirDisponibilitesDansIntervalleQuery(medecinId, start, end);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibilitesDansIntervalleAsync(medecinId, start, end))
                .ReturnsAsync(new List<Disponibilite>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
