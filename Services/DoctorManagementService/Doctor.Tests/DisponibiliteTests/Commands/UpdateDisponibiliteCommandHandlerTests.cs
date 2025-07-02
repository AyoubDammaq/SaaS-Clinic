using Doctor.Application.AvailibilityServices.Commands.UpdateDisponibilite;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Commands
{
    public class UpdateDisponibiliteCommandHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly UpdateDisponibiliteCommandHandler _handler;

        public UpdateDisponibiliteCommandHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new UpdateDisponibiliteCommandHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenDisponibiliteIsNull()
        {
            // Arrange
            var command = new UpdateDisponibiliteCommand(Guid.NewGuid(), null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenHeureDebutIsAfterOrEqualHeureFin()
        {
            // Arrange
            var dispoId = Guid.NewGuid();
            var dispo = new Disponibilite
            {
                Id = dispoId,
                Jour = DayOfWeek.Monday,
                HeureDebut = new TimeSpan(14, 0, 0),
                HeureFin = new TimeSpan(12, 0, 0),
                MedecinId = Guid.NewGuid()
            };
            var command = new UpdateDisponibiliteCommand(dispoId, dispo);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenChevauchementExiste()
        {
            // Arrange
            var dispoId = Guid.NewGuid();
            var dispo = new Disponibilite
            {
                Id = dispoId,
                Jour = DayOfWeek.Tuesday,
                HeureDebut = new TimeSpan(10, 0, 0),
                HeureFin = new TimeSpan(12, 0, 0),
                MedecinId = Guid.NewGuid()
            };
            var command = new UpdateDisponibiliteCommand(dispoId, dispo);

            _disponibiliteRepositoryMock
                .Setup(r => r.VerifieChevauchementAsync(It.IsAny<Disponibilite>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldCallUpdateDisponibiliteAsync_And_AddEvent_WhenValid()
        {
            // Arrange
            var dispoId = Guid.NewGuid();
            var dispo = new Disponibilite
            {
                Id = dispoId,
                Jour = DayOfWeek.Wednesday,
                HeureDebut = new TimeSpan(8, 0, 0),
                HeureFin = new TimeSpan(10, 0, 0),
                MedecinId = Guid.NewGuid()
            };
            var command = new UpdateDisponibiliteCommand(dispoId, dispo);

            _disponibiliteRepositoryMock
                .Setup(r => r.VerifieChevauchementAsync(It.IsAny<Disponibilite>()))
                .ReturnsAsync(false);

            _disponibiliteRepositoryMock
                .Setup(r => r.UpdateDisponibiliteAsync(dispoId, dispo))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _disponibiliteRepositoryMock.Verify(r => r.UpdateDisponibiliteAsync(dispoId, dispo), Times.Once);
            Assert.Contains(dispo.DomainEvents, e => e.GetType().Name == "DisponibiliteModifiee" || e.GetType().Name.Contains("Disponibilite"));
        }

        [Fact]
        public async Task Handle_ShouldNotThrow_WhenNoChevauchement_AndValidTimes()
        {
            // Arrange
            var dispoId = Guid.NewGuid();
            var dispo = new Disponibilite
            {
                Id = dispoId,
                Jour = DayOfWeek.Friday,
                HeureDebut = new TimeSpan(9, 0, 0),
                HeureFin = new TimeSpan(11, 0, 0),
                MedecinId = Guid.NewGuid()
            };
            var command = new UpdateDisponibiliteCommand(dispoId, dispo);

            _disponibiliteRepositoryMock
                .Setup(r => r.VerifieChevauchementAsync(It.IsAny<Disponibilite>()))
                .ReturnsAsync(false);

            _disponibiliteRepositoryMock
                .Setup(r => r.UpdateDisponibiliteAsync(dispoId, dispo))
                .Returns(Task.CompletedTask);

            // Act
            var exception = await Record.ExceptionAsync(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            Assert.Null(exception);
            _disponibiliteRepositoryMock.Verify(r => r.UpdateDisponibiliteAsync(dispoId, dispo), Times.Once);
        }
    }
}
