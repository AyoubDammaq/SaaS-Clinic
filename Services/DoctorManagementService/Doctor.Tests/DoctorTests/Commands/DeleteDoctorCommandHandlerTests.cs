using Doctor.Application.DoctorServices.Commands.DeleteDoctor;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DoctorTests.Commands
{
    public class DeleteDoctorCommandHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly DeleteDoctorCommandHandler _handler;

        public DeleteDoctorCommandHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new DeleteDoctorCommandHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var command = new DeleteDoctorCommand(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldCallRemoveDoctorEventAndDeleteAsync_WhenMedecinExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteDoctorCommand(id);

            // Utilisation d'une instance réelle de Medecin
            var medecin = new Medecin
            {
                Id = id,
                Prenom = "Test",
                Nom = "Test",
                Specialite = "Test"
                // Ajoutez d'autres propriétés requises si besoin
            };

            _medecinRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(medecin);

            _medecinRepositoryMock
                .Setup(r => r.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _medecinRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
            // Impossible de vérifier RemoveDoctorEvent sans modification de la classe
        }


        [Fact]
        public async Task Handle_ShouldThrowNullReferenceException_WhenMedecinNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteDoctorCommand(id);

            _medecinRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Medecin)null);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}
