using Doctor.Application.AvailibilityServices.Commands.SupprimerDisponibilite;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Commands
{
    public class SupprimerDisponibiliteCommandHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly SupprimerDisponibiliteCommandHandler _handler;

        public SupprimerDisponibiliteCommandHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new SupprimerDisponibiliteCommandHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenDisponibiliteIdIsEmpty()
        {
            // Arrange
            var command = new SupprimerDisponibiliteCommand(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenDisponibiliteNotFound()
        {
            // Arrange
            var dispoId = Guid.NewGuid();
            var command = new SupprimerDisponibiliteCommand(dispoId);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibiliteParIdAsync(dispoId))
                .ReturnsAsync((Disponibilite)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldCallSupprimerDisponibiliteEvent_And_SupprimerDisponibiliteAsync()
        {
            // Arrange
            var dispoId = Guid.NewGuid();
            var command = new SupprimerDisponibiliteCommand(dispoId);

            var dispoMock = new Mock<Disponibilite>();
            dispoMock.Setup(d => d.SupprimerDisponibiliteEvent());

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibiliteParIdAsync(dispoId))
                .ReturnsAsync(dispoMock.Object);

            _disponibiliteRepositoryMock
                .Setup(r => r.SupprimerDisponibiliteAsync(dispoId))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            dispoMock.Verify(d => d.SupprimerDisponibiliteEvent(), Times.Once);
            _disponibiliteRepositoryMock.Verify(r => r.SupprimerDisponibiliteAsync(dispoId), Times.Once);
        }
    }
}
