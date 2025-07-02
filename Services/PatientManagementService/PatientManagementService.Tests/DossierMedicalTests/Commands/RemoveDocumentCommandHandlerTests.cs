using FluentAssertions;
using Moq;
using PatientManagementService.Application.DossierMedicalService.Commands.RemoveDocument;
using PatientManagementService.Domain.Interfaces;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Tests.DossierMedicalTests.Commands
{
    public class RemoveDocumentCommandHandlerTests
    {
        private readonly Mock<IDossierMedicalRepository> _dossierMedicalRepositoryMock;
        private readonly RemoveDocumentCommandHandler _handler;

        public RemoveDocumentCommandHandlerTests()
        {
            _dossierMedicalRepositoryMock = new Mock<IDossierMedicalRepository>();
            _handler = new RemoveDocumentCommandHandler(_dossierMedicalRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenDocumentNotFound()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var command = new RemoveDocumentCommand(documentId);
            _dossierMedicalRepositoryMock.Setup(r => r.GetDocumentByIdAsync(documentId))
                .ReturnsAsync((Document?)null);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dossierMedicalRepositoryMock.Verify(r => r.RemoveDocumentAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallDetacherDocumentEventAndRemove_WhenDocumentExists()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var documentMock = new Mock<Document>();
            documentMock.SetupAllProperties();
            documentMock.Object.Id = documentId;

            var command = new RemoveDocumentCommand(documentId);
            _dossierMedicalRepositoryMock.Setup(r => r.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(documentMock.Object);
            _dossierMedicalRepositoryMock.Setup(r => r.RemoveDocumentAsync(documentId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            documentMock.Verify(d => d.DetacherDocumentEvent(documentMock.Object), Times.Once);
            _dossierMedicalRepositoryMock.Verify(r => r.RemoveDocumentAsync(documentId), Times.Once);
        }
    }
}
