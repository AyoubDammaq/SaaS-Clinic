using ConsultationManagementService.Application.Commands.DeleteDocumentMedical;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;

namespace ConsultationManagementService.Tests.Commands
{
    public class DeleteDocumentMedicalCommandHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly Mock<ILogger<DeleteDocumentMedicalCommandHandler>> _loggerMock;
        private readonly DeleteDocumentMedicalCommandHandler _handler;

        public DeleteDocumentMedicalCommandHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _loggerMock = new Mock<ILogger<DeleteDocumentMedicalCommandHandler>>();
            _handler = new DeleteDocumentMedicalCommandHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_IdIsEmpty()
        {
            var command = new DeleteDocumentMedicalCommand(Guid.Empty);
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*identifiant*");
        }

        [Fact]
        public async Task Handle_Should_ThrowNullReferenceException_When_DocumentNotFound()
        {
            var id = Guid.NewGuid();
            var command = new DeleteDocumentMedicalCommand(id);
            _repositoryMock.Setup(r => r.GetDocumentMedicalByIdAsync(id)).ReturnsAsync((DocumentMedical?)null);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task Handle_Should_CallRemoveMedicalDocumentEvent_AndDeleteDocumentMedicalAsync_AndReturnResult()
        {
            var id = Guid.NewGuid();
            var command = new DeleteDocumentMedicalCommand(id);
            var document = new Mock<DocumentMedical>();
            document.Setup(d => d.RemoveMedicalDocumentEvent()).Verifiable();
            _repositoryMock.Setup(r => r.GetDocumentMedicalByIdAsync(id)).ReturnsAsync(document.Object);
            _repositoryMock.Setup(r => r.DeleteDocumentMedicalAsync(id)).ReturnsAsync(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
            document.Verify(d => d.RemoveMedicalDocumentEvent(), Times.Once);
            _repositoryMock.Verify(r => r.GetDocumentMedicalByIdAsync(id), Times.Once);
            _repositoryMock.Verify(r => r.DeleteDocumentMedicalAsync(id), Times.Once);
        }
    }
}
