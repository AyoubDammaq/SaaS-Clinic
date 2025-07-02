using ConsultationManagementService.Application.Queries.GetDocumentMedicalById;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Queries
{
    public class GetDocumentMedicalByIdQueryHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly GetDocumentMedicalByIdQueryHandler _handler;

        public GetDocumentMedicalByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _handler = new GetDocumentMedicalByIdQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_IdIsEmpty()
        {
            // Arrange
            var query = new GetDocumentMedicalByIdQuery(Guid.Empty);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*identifiant*");
        }

        [Fact]
        public async Task Handle_Should_ReturnDocumentMedical_When_DocumentExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetDocumentMedicalByIdQuery(id);
            var document = new DocumentMedical { Id = id };

            _repositoryMock.Setup(r => r.GetDocumentMedicalByIdAsync(id)).ReturnsAsync(document);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(document);
            _repositoryMock.Verify(r => r.GetDocumentMedicalByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnNull_When_DocumentDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetDocumentMedicalByIdQuery(id);

            _repositoryMock.Setup(r => r.GetDocumentMedicalByIdAsync(id)).ReturnsAsync((DocumentMedical?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _repositoryMock.Verify(r => r.GetDocumentMedicalByIdAsync(id), Times.Once);
        }
    }
}
