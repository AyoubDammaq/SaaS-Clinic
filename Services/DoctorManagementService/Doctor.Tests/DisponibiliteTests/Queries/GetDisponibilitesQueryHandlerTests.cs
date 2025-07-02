using Doctor.Application.AvailibilityServices.Queries.GetDisponibilites;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Queries
{
    public class GetDisponibilitesQueryHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly GetDisponibilitesQueryHandler _handler;

        public GetDisponibilitesQueryHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new GetDisponibilitesQueryHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDisponibilites_WhenRepositoryReturnsList()
        {
            // Arrange
            var disponibilites = new List<Disponibilite>
            {
                new Disponibilite
                {
                    Id = Guid.NewGuid(),
                    Jour = DayOfWeek.Monday,
                    HeureDebut = new TimeSpan(8, 0, 0),
                    HeureFin = new TimeSpan(12, 0, 0),
                    MedecinId = Guid.NewGuid()
                }
            };
            var query = new GetDisponibilitesQuery();

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirToutesDisponibilitesAsync())
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
            var query = new GetDisponibilitesQuery();

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirToutesDisponibilitesAsync())
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
            var query = new GetDisponibilitesQuery();

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirToutesDisponibilitesAsync())
                .ReturnsAsync(new List<Disponibilite>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
