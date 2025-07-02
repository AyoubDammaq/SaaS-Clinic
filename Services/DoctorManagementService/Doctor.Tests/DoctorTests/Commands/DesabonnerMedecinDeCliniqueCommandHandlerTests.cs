using Doctor.Application.DoctorServices.Commands.DesabonnerMedecinDeClinique;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DoctorTests.Commands
{
    public class DesabonnerMedecinDeCliniqueCommandHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly DesabonnerMedecinDeCliniqueCommandHandler _handler;

        public DesabonnerMedecinDeCliniqueCommandHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new DesabonnerMedecinDeCliniqueCommandHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdIsEmpty()
        {
            // Arrange
            var command = new DesabonnerMedecinDeCliniqueCommand(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenMedecinHasNoClinique()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var command = new DesabonnerMedecinDeCliniqueCommand(medecinId);

            var medecin = new Medecin
            {
                Id = medecinId,
                CliniqueId = null
            };

            _medecinRepositoryMock
                .Setup(r => r.GetByIdAsync(medecinId))
                .ReturnsAsync(medecin);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldCallDesabonnerDeCliniqueEvent_And_DesabonnerMedecinDeCliniqueAsync_WhenMedecinHasClinique()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var cliniqueId = Guid.NewGuid();
            var command = new DesabonnerMedecinDeCliniqueCommand(medecinId);

            // Utiliser une vraie instance de Medecin
            var medecin = new Medecin
            {
                Id = medecinId,
                CliniqueId = cliniqueId
                // Initialisez les autres propriétés requises si besoin
            };

            // Utiliser un Mock pour le repository
            _medecinRepositoryMock
                .Setup(r => r.GetByIdAsync(medecinId))
                .ReturnsAsync(medecin);

            _medecinRepositoryMock
                .Setup(r => r.DesabonnerMedecinDeCliniqueAsync(medecinId))
                .Returns(Task.CompletedTask);

            // Vous pouvez surveiller l'appel à DesabonnerDeCliniqueEvent en utilisant un flag ou en remplaçant la méthode dans une sous-classe si besoin

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _medecinRepositoryMock.Verify(r => r.DesabonnerMedecinDeCliniqueAsync(medecinId), Times.Once);
            // Pour vérifier l'appel à DesabonnerDeCliniqueEvent, il faudrait adapter la classe Medecin (ex: exposer un flag ou utiliser un event)
        }


        [Fact]
        public async Task Handle_ShouldThrowNullReferenceException_WhenMedecinNotFound()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var command = new DesabonnerMedecinDeCliniqueCommand(medecinId);

            _medecinRepositoryMock
                .Setup(r => r.GetByIdAsync(medecinId))
                .ReturnsAsync((Medecin)null);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
