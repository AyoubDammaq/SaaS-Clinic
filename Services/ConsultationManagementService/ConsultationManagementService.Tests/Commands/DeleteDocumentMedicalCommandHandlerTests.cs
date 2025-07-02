using ConsultationManagementService.Application.Commands.DeleteDocumentMedical;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Commands
{
    public class DeleteDocumentMedicalCommandHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly DeleteDocumentMedicalCommandHandler _handler;

        public DeleteDocumentMedicalCommandHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _handler = new DeleteDocumentMedicalCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_IdIsEmpty()
        {
            // Arrange
            var command = new DeleteDocumentMedicalCommand(Guid.Empty);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*identifiant*");
        }

        [Fact]
        public async Task Handle_Should_ThrowException_When_DocumentNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteDocumentMedicalCommand(id);
            _repositoryMock.Setup(r => r.GetDocumentMedicalByIdAsync(id)).ReturnsAsync((DocumentMedical?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task Handle_Should_CallDeleteDocumentMedicalAsync_AndReturnResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteDocumentMedicalCommand(id);
            var document = new DocumentMedical { Id = id };
            _repositoryMock.Setup(r => r.GetDocumentMedicalByIdAsync(id)).ReturnsAsync(document);
            _repositoryMock.Setup(r => r.DeleteDocumentMedicalAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _repositoryMock.Verify(r => r.GetDocumentMedicalByIdAsync(id), Times.Once);
            _repositoryMock.Verify(r => r.DeleteDocumentMedicalAsync(id), Times.Once);
        }
    }
}
