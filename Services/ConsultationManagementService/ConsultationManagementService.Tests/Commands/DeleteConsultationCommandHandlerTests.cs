using ConsultationManagementService.Application.Commands.DeleteConsultation;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Commands
{
    public class DeleteConsultationCommandHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly DeleteConsultationCommandHandler _handler;

        public DeleteConsultationCommandHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _handler = new DeleteConsultationCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_IdIsEmpty()
        {
            // Arrange
            var command = new DeleteConsultationCommand(Guid.Empty);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*identifiant*");
        }

        [Fact]
        public async Task Handle_Should_ThrowException_When_ConsultationNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteConsultationCommand(id);
            _repositoryMock.Setup(r => r.GetConsultationByIdAsync(id)).ReturnsAsync((Consultation?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task Handle_Should_CallDeleteConsultationAsync_AndReturnResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteConsultationCommand(id);
            var consultation = new Consultation { Id = id };
            _repositoryMock.Setup(r => r.GetConsultationByIdAsync(id)).ReturnsAsync(consultation);
            _repositoryMock.Setup(r => r.DeleteConsultationAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _repositoryMock.Verify(r => r.GetConsultationByIdAsync(id), Times.Once);
            _repositoryMock.Verify(r => r.DeleteConsultationAsync(id), Times.Once);
        }
    }
}
