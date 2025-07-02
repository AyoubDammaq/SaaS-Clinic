using FluentAssertions;
using Moq;
using PatientManagementService.Application.DossierMedicalService.Commands.DeleteDossierMedical;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.DossierMedicalTests.Commands
{
    public class DeleteDossierMedicalCommandHandlerTests
    {
        private readonly Mock<IDossierMedicalRepository> _dossierMedicalRepositoryMock;
        private readonly DeleteDossierMedicalCommandHandler _handler;

        public DeleteDossierMedicalCommandHandlerTests()
        {
            _dossierMedicalRepositoryMock = new Mock<IDossierMedicalRepository>();
            _handler = new DeleteDossierMedicalCommandHandler(_dossierMedicalRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenDossierMedicalNotFound()
        {
            // Arrange
            var dossierId = Guid.NewGuid();
            var command = new DeleteDossierMedicalCommand(dossierId);
            _dossierMedicalRepositoryMock.Setup(r => r.GetDossierMedicalByIdAsync(dossierId))
                .ReturnsAsync((DossierMedical?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<Exception>();
            ex.Which.Message.Should().Be("Dossier médical not found");
            _dossierMedicalRepositoryMock.Verify(r => r.DeleteDossierMedicalAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallSupprimerDossierMedicalEventAndDelete_WhenDossierMedicalExists()
        {
            // Arrange
            var dossierId = Guid.NewGuid();
            var dossierMedicalMock = new Mock<DossierMedical>();
            dossierMedicalMock.SetupAllProperties();
            dossierMedicalMock.Object.Id = dossierId;

            var command = new DeleteDossierMedicalCommand(dossierId);
            _dossierMedicalRepositoryMock.Setup(r => r.GetDossierMedicalByIdAsync(dossierId))
                .ReturnsAsync(dossierMedicalMock.Object);
            _dossierMedicalRepositoryMock.Setup(r => r.DeleteDossierMedicalAsync(dossierId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            dossierMedicalMock.Verify(d => d.SupprimerDossierMedicalEvent(), Times.Once);
            _dossierMedicalRepositoryMock.Verify(r => r.DeleteDossierMedicalAsync(dossierId), Times.Once);
        }
    }
}
