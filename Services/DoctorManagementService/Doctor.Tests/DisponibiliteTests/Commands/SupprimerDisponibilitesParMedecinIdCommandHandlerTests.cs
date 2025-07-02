using Doctor.Application.AvailibilityServices.Commands.SupprimerDisponibilitesParMedecinId;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Commands
{
    public class SupprimerDisponibilitesParMedecinIdCommandHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly SupprimerDisponibilitesParMedecinIdCommandHandler _handler;

        public SupprimerDisponibilitesParMedecinIdCommandHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new SupprimerDisponibilitesParMedecinIdCommandHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdIsEmpty()
        {
            // Arrange
            var command = new SupprimerDisponibilitesParMedecinIdCommand(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldCallSupprimerToutesPourMedecinEvent_And_SupprimerDisponibilitesParMedecinIdAsync()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var disponibilites = new List<Disponibilite>
            {
                new Disponibilite { Id = Guid.NewGuid(), MedecinId = medecinId, Jour = DayOfWeek.Monday, HeureDebut = new TimeSpan(8,0,0), HeureFin = new TimeSpan(12,0,0) }
            };
            var command = new SupprimerDisponibilitesParMedecinIdCommand(medecinId);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirDisponibilitesParMedecinIdAsync(medecinId))
                .ReturnsAsync(disponibilites);

            _disponibiliteRepositoryMock
                .Setup(r => r.SupprimerDisponibilitesParMedecinIdAsync(medecinId))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _disponibiliteRepositoryMock.Verify(r => r.ObtenirDisponibilitesParMedecinIdAsync(medecinId), Times.Once);
            _disponibiliteRepositoryMock.Verify(r => r.SupprimerDisponibilitesParMedecinIdAsync(medecinId), Times.Once);
            // L'appel à Disponibilite.SupprimerToutesPourMedecinEvent ne peut pas être vérifié directement car c'est une méthode statique.
        }
    }
}
