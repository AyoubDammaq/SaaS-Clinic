using Doctor.Application.AvailibilityServices.Queries.GetTotalAvailableTime;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Queries
{
    public class GetTotalAvailableTimeQueryHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly GetTotalAvailableTimeQueryHandler _handler;

        public GetTotalAvailableTimeQueryHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new GetTotalAvailableTimeQueryHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdIsEmpty()
        {
            // Arrange
            var query = new GetTotalAvailableTimeQuery(Guid.Empty, DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenDateDebutIsAfterOrEqualDateFin()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var dateDebut = DateTime.Now.AddDays(1);
            var dateFin = DateTime.Now;
            var query = new GetTotalAvailableTimeQuery(medecinId, dateDebut, dateFin);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnTimeSpan_WhenRepositoryReturnsValue()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var dateDebut = new DateTime(2024, 6, 1, 8, 0, 0);
            var dateFin = new DateTime(2024, 6, 1, 18, 0, 0);
            var expected = TimeSpan.FromHours(5);
            var query = new GetTotalAvailableTimeQuery(medecinId, dateDebut, dateFin);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirTempsTotalDisponibleAsync(medecinId, dateDebut, dateFin))
                .ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
