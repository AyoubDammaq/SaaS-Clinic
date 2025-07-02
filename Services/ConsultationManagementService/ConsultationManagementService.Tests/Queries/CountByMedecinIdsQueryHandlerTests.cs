using ConsultationManagementService.Application.Queries.CountByMedecinIds;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Queries
{
    public class CountByMedecinIdsQueryHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly CountByMedecinIdsQueryHandler _handler;

        public CountByMedecinIdsQueryHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _handler = new CountByMedecinIdsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_MedecinIdsIsNull()
        {
            // Arrange
            var query = new CountByMedecinIdsQuery(null);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*médecins*");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_MedecinIdsIsEmpty()
        {
            // Arrange
            var query = new CountByMedecinIdsQuery(new List<Guid>());

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*médecins*");
        }

        [Fact]
        public async Task Handle_Should_ReturnCount_When_MedecinIdsIsValid()
        {
            // Arrange
            var medecinIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var query = new CountByMedecinIdsQuery(medecinIds);
            _repositoryMock.Setup(r => r.CountByMedecinIdsAsync(medecinIds)).ReturnsAsync(5);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(5);
            _repositoryMock.Verify(r => r.CountByMedecinIdsAsync(medecinIds), Times.Once);
        }
    }
}
