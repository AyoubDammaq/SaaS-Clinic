using FluentAssertions;
using Moq;
using RDV.Application.Queries.CountDistinctPatientsByMedecinIds;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Queries
{
    public class CountDistinctPatientsByMedecinIdsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnCount_WhenMedecinIdsAreValid()
        {
            // Arrange
            var medecinIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var expectedCount = 3;
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.CountDistinctPatientsByMedecinIdsAsync(medecinIds))
                .ReturnsAsync(expectedCount);

            var handler = new CountDistinctPatientsByMedecinIdsQueryHandler(repoMock.Object);
            var query = new CountDistinctPatientsByMedecinIdsQuery(medecinIds);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(expectedCount);
            repoMock.Verify(r => r.CountDistinctPatientsByMedecinIdsAsync(medecinIds), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdsIsNull()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new CountDistinctPatientsByMedecinIdsQueryHandler(repoMock.Object);
            var query = new CountDistinctPatientsByMedecinIdsQuery(null);

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
            var handler = new CountDistinctPatientsByMedecinIdsQueryHandler(repoMock.Object);
            var query = new CountDistinctPatientsByMedecinIdsQuery(new List<Guid>());

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*La liste des identifiants de médecins ne peut pas être vide*");
        }
    }
}
