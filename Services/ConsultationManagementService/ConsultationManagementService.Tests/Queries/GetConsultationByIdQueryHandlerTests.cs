using ConsultationManagementService.Application.Queries.GetConsultationById;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Queries
{
    public class GetConsultationByIdQueryHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly GetConsultationByIdQueryHandler _handler;

        public GetConsultationByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _handler = new GetConsultationByIdQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_IdIsEmpty()
        {
            // Arrange
            var query = new GetConsultationByIdQuery(Guid.Empty);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*identifiant*");
        }

        [Fact]
        public async Task Handle_Should_ReturnConsultation_When_ConsultationExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetConsultationByIdQuery(id);
            var consultation = new Consultation { Id = id };

            _repositoryMock.Setup(r => r.GetConsultationByIdAsync(id)).ReturnsAsync(consultation);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(consultation);
            _repositoryMock.Verify(r => r.GetConsultationByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnNull_When_ConsultationDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetConsultationByIdQuery(id);

            _repositoryMock.Setup(r => r.GetConsultationByIdAsync(id)).ReturnsAsync((Consultation?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _repositoryMock.Verify(r => r.GetConsultationByIdAsync(id), Times.Once);
        }
    }
}
