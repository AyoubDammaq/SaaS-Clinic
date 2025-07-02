using Doctor.Application.AvailibilityServices.Queries.GetDisponibilitesByMedecinId;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Queries
{
    public class GetDisponibilitesByMedecinIdQueryHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly GetDisponibilitesByMedecinIdQueryHandler _handler;

        public GetDisponibilitesByMedecinIdQueryHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new GetDisponibilitesByMedecinIdQueryHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdIsEmpty()
        {
            // Arrange
            var query = new GetDisponibilitesByMedecinIdQuery(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnDisponibilites_WhenRepositoryReturnsList()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var disponibilites = new List<Disponibilite>
            {
                new Disponibilite
                {
                    Id = Guid.NewGuid(),
                    Jour = DayOfWeek.Wednesday,
                    HeureDebut = new TimeSpan(9, 0, 0),
                    HeureFin = new TimeSpan(12, 0, 0),
                    MedecinId = medecinId
                }
            };
            var query = new GetDisponibilitesByMedecinIdQuery(medecinId);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibilitesParMedecinIdAsync(medecinId))
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
            var query = new GetDisponibilitesByMedecinIdQuery(medecinId);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibilitesParMedecinIdAsync(medecinId))
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
            var query = new GetDisponibilitesByMedecinIdQuery(medecinId);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibilitesParMedecinIdAsync(medecinId))
                .ReturnsAsync(new List<Disponibilite>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
