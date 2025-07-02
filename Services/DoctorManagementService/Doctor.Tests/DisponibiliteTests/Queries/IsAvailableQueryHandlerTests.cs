using Doctor.Application.AvailibilityServices.Queries.IsAvailable;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Queries
{
    public class IsAvailableQueryHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly IsAvailableQueryHandler _handler;

        public IsAvailableQueryHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new IsAvailableQueryHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdIsEmpty()
        {
            // Arrange
            var query = new IsAvailableQuery(Guid.Empty, DateTime.Now);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenMedecinIsAvailable()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var dateTime = new DateTime(2024, 6, 1, 10, 0, 0);
            var query = new IsAvailableQuery(medecinId, dateTime);

            _disponibiliteRepositoryMock
                .Setup(r => r.EstDisponibleAsync(medecinId, dateTime))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenMedecinIsNotAvailable()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var dateTime = new DateTime(2024, 6, 1, 15, 0, 0);
            var query = new IsAvailableQuery(medecinId, dateTime);

            _disponibiliteRepositoryMock
                .Setup(r => r.EstDisponibleAsync(medecinId, dateTime))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result);
        }
    }
}
