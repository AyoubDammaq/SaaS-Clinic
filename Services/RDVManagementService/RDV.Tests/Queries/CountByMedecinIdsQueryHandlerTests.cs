using FluentAssertions;
using Moq;
using RDV.Application.Queries.CountByMedecinIds;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Queries
{
    public class CountByMedecinIdsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnCount_WhenMedecinIdsAreValid()
        {
            // Arrange
            var medecinIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var expectedCount = 5;
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.CountByMedecinIdsAsync(medecinIds))
                .ReturnsAsync(expectedCount);

            var handler = new CountByMedecinIdsQueryHandler(repoMock.Object);
            var query = new CountByMedecinIdsQuery(medecinIds);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(expectedCount);
            repoMock.Verify(r => r.CountByMedecinIdsAsync(medecinIds), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdsIsNull()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new CountByMedecinIdsQueryHandler(repoMock.Object);
            var query = new CountByMedecinIdsQuery(null);

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*La liste des identifiants de médecins ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdsIsEmpty()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new CountByMedecinIdsQueryHandler(repoMock.Object);
            var query = new CountByMedecinIdsQuery(new List<Guid>());

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*La liste des identifiants de médecins ne peut pas être vide*");
        }
    }
}
