using Doctor.Application.AvailibilityServices.Queries.GetDisponibilitesByMedecinIdAndJour;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Queries
{
    public class GetDisponibilitesByMedecinIdAndJourQueryHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly GetDisponibilitesByMedecinIdAndJourQueryHandler _handler;

        public GetDisponibilitesByMedecinIdAndJourQueryHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new GetDisponibilitesByMedecinIdAndJourQueryHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdIsEmpty()
        {
            // Arrange
            var query = new GetDisponibilitesByMedecinIdAndJourQuery(Guid.Empty, DayOfWeek.Monday);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnDisponibilites_WhenRepositoryReturnsList()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var jour = DayOfWeek.Tuesday;
            var disponibilites = new List<Disponibilite>
            {
                new Disponibilite
                {
                    Id = Guid.NewGuid(),
                    Jour = jour,
                    HeureDebut = new TimeSpan(10, 0, 0),
                    HeureFin = new TimeSpan(12, 0, 0),
                    MedecinId = medecinId
                }
            };
            var query = new GetDisponibilitesByMedecinIdAndJourQuery(medecinId, jour);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibilitesParJourAsync(medecinId, jour))
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
            var jour = DayOfWeek.Wednesday;
            var query = new GetDisponibilitesByMedecinIdAndJourQuery(medecinId, jour);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibilitesParJourAsync(medecinId, jour))
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
            var jour = DayOfWeek.Friday;
            var query = new GetDisponibilitesByMedecinIdAndJourQuery(medecinId, jour);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibilitesParJourAsync(medecinId, jour))
                .ReturnsAsync(new List<Disponibilite>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
