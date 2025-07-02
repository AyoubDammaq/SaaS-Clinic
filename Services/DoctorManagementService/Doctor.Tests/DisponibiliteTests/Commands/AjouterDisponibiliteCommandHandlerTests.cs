using Doctor.Application.AvailibilityServices.Commands.AjouterDisponibilite;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Commands
{
    public class AjouterDisponibiliteCommandHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly AjouterDisponibiliteCommandHandler _handler;

        public AjouterDisponibiliteCommandHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new AjouterDisponibiliteCommandHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenDisponibiliteIsNull()
        {
            // Arrange
            var command = new AjouterDisponibiliteCommand(null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenHeureDebutIsAfterOrEqualHeureFin()
        {
            // Arrange
            var dispo = new Disponibilite
            {
                Jour = DayOfWeek.Monday,
                HeureDebut = new TimeSpan(14, 0, 0),
                HeureFin = new TimeSpan(12, 0, 0),
                MedecinId = Guid.NewGuid()
            };
            var command = new AjouterDisponibiliteCommand(dispo);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenChevauchementExiste()
        {
            // Arrange
            var dispo = new Disponibilite
            {
                Jour = DayOfWeek.Tuesday,
                HeureDebut = new TimeSpan(10, 0, 0),
                HeureFin = new TimeSpan(12, 0, 0),
                MedecinId = Guid.NewGuid()
            };
            var command = new AjouterDisponibiliteCommand(dispo);

            _disponibiliteRepositoryMock
                .Setup(r => r.VerifieChevauchementAsync(dispo))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldCallAjouterDisponibiliteAsync_And_AddEvent_WhenValid()
        {
            // Arrange
            var dispo = new Disponibilite
            {
                Jour = DayOfWeek.Wednesday,
                HeureDebut = new TimeSpan(8, 0, 0),
                HeureFin = new TimeSpan(10, 0, 0),
                MedecinId = Guid.NewGuid()
            };
            var command = new AjouterDisponibiliteCommand(dispo);

            _disponibiliteRepositoryMock
                .Setup(r => r.VerifieChevauchementAsync(dispo))
                .ReturnsAsync(false);

            _disponibiliteRepositoryMock
                .Setup(r => r.AjouterDisponibiliteAsync(dispo))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _disponibiliteRepositoryMock.Verify(r => r.AjouterDisponibiliteAsync(dispo), Times.Once);
            Assert.Contains(dispo.DomainEvents, e => e.GetType().Name == "DisponibiliteCreee" || e.GetType().Name.Contains("Disponibilite"));
        }

        [Fact]
        public async Task Handle_ShouldNotThrow_WhenNoChevauchement_AndValidTimes()
        {
            // Arrange
            var dispo = new Disponibilite
            {
                Jour = DayOfWeek.Friday,
                HeureDebut = new TimeSpan(9, 0, 0),
                HeureFin = new TimeSpan(11, 0, 0),
                MedecinId = Guid.NewGuid()
            };
            var command = new AjouterDisponibiliteCommand(dispo);

            _disponibiliteRepositoryMock
                .Setup(r => r.VerifieChevauchementAsync(dispo))
                .ReturnsAsync(false);

            _disponibiliteRepositoryMock
                .Setup(r => r.AjouterDisponibiliteAsync(dispo))
                .Returns(Task.CompletedTask);

            // Act
            var exception = await Record.ExceptionAsync(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            Assert.Null(exception);
            _disponibiliteRepositoryMock.Verify(r => r.AjouterDisponibiliteAsync(dispo), Times.Once);
        }
    }
}
