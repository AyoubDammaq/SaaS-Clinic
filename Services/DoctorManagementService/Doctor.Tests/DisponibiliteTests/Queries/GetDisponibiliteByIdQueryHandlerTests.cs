using Doctor.Application.AvailibilityServices.Queries.GetDisponibiliteById;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Queries
{
    public class GetDisponibiliteByIdQueryHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly GetDisponibiliteByIdQueryHandler _handler;

        public GetDisponibiliteByIdQueryHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new GetDisponibiliteByIdQueryHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenDisponibiliteIdIsEmpty()
        {
            // Arrange
            var query = new GetDisponibiliteByIdQuery(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnDisponibilite_WhenFound()
        {
            // Arrange
            var dispoId = Guid.NewGuid();
            var dispo = new Disponibilite
            {
                Id = dispoId,
                Jour = DayOfWeek.Friday,
                HeureDebut = new TimeSpan(9, 0, 0),
                HeureFin = new TimeSpan(12, 0, 0),
                MedecinId = Guid.NewGuid()
            };
            var query = new GetDisponibiliteByIdQuery(dispoId);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibiliteParIdAsync(dispoId))
                .ReturnsAsync(dispo);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dispoId, result.Id);
            Assert.Equal(dispo.Jour, result.Jour);
            Assert.Equal(dispo.HeureDebut, result.HeureDebut);
            Assert.Equal(dispo.HeureFin, result.HeureFin);
            Assert.Equal(dispo.MedecinId, result.MedecinId);
        }

        [Fact]
        public async Task Handle_ShouldReturnNewDisponibilite_WhenNotFound()
        {
            // Arrange
            var dispoId = Guid.NewGuid();
            var query = new GetDisponibiliteByIdQuery(dispoId);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibiliteParIdAsync(dispoId))
                .ReturnsAsync((Disponibilite)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Disponibilite>(result);
            //Assert.Equal(default, result.Id);
            Assert.Equal(default, result.Jour);
            Assert.Equal(default, result.HeureDebut);
            Assert.Equal(default, result.HeureFin);
            Assert.Equal(default, result.MedecinId);
        }
    }
}
