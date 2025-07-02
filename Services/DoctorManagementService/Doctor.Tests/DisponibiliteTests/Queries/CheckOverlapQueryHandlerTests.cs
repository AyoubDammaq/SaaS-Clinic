using Doctor.Application.AvailibilityServices.Queries.CheckOverlap;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Queries
{
    public class CheckOverlapQueryHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly CheckOverlapQueryHandler _handler;

        public CheckOverlapQueryHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new CheckOverlapQueryHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenDispoIsNull()
        {
            // Arrange
            var query = new CheckOverlapQuery(null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenOverlapExists()
        {
            // Arrange
            var dispo = new Disponibilite
            {
                Jour = DayOfWeek.Monday,
                HeureDebut = new TimeSpan(9, 0, 0),
                HeureFin = new TimeSpan(12, 0, 0),
                MedecinId = Guid.NewGuid()
            };
            var query = new CheckOverlapQuery(dispo);

            _disponibiliteRepositoryMock
                .Setup(r => r.VerifieChevauchementAsync(dispo))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenNoOverlap()
        {
            // Arrange
            var dispo = new Disponibilite
            {
                Jour = DayOfWeek.Tuesday,
                HeureDebut = new TimeSpan(14, 0, 0),
                HeureFin = new TimeSpan(16, 0, 0),
                MedecinId = Guid.NewGuid()
            };
            var query = new CheckOverlapQuery(dispo);

            _disponibiliteRepositoryMock
                .Setup(r => r.VerifieChevauchementAsync(dispo))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result);
        }
    }
}
