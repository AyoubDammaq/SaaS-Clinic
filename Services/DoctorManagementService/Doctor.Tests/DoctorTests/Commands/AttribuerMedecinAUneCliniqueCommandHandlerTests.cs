using Doctor.Application.DoctorServices.Commands.AttribuerMedecinAUneClinique;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DoctorTests.Commands
{
    public class AttribuerMedecinAUneCliniqueCommandHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly AttribuerMedecinAUneCliniqueCommandHandler _handler;

        public AttribuerMedecinAUneCliniqueCommandHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new AttribuerMedecinAUneCliniqueCommandHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenIdsAreEmpty()
        {
            // Arrange
            var command = new AttribuerMedecinAUneCliniqueCommand(Guid.Empty, Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldCallAssignerCliniqueEvent_And_AttribuerMedecinAUneCliniqueAsync_WhenMedecinExists()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var cliniqueId = Guid.NewGuid();
            var command = new AttribuerMedecinAUneCliniqueCommand(medecinId, cliniqueId);

            var medecinTestable = new MedecinTestable();

            _medecinRepositoryMock
                .Setup(r => r.GetByIdAsync(medecinId))
                .ReturnsAsync(medecinTestable);

            _medecinRepositoryMock
                .Setup(r => r.AttribuerMedecinAUneCliniqueAsync(medecinId, cliniqueId))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(medecinTestable.AssignerCliniqueEventCalled);
            Assert.Equal(cliniqueId, medecinTestable.DernierCliniqueId);
            _medecinRepositoryMock.Verify(r => r.AttribuerMedecinAUneCliniqueAsync(medecinId, cliniqueId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNullReferenceException_WhenMedecinNotFound()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var cliniqueId = Guid.NewGuid();
            var command = new AttribuerMedecinAUneCliniqueCommand(medecinId, cliniqueId);

            _medecinRepositoryMock
                .Setup(r => r.GetByIdAsync(medecinId))
                .ReturnsAsync((Medecin)null);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
        // Classe de test interne
        private class MedecinTestable : Medecin
        {
            public bool AssignerCliniqueEventCalled { get; private set; }
            public Guid? DernierCliniqueId { get; private set; }

            public override void AssignerCliniqueEvent(Guid cliniqueId)
            {
                AssignerCliniqueEventCalled = true;
                DernierCliniqueId = cliniqueId;
            }
        }
    }
}
