using FluentAssertions;
using Moq;
using PatientManagementService.Application.PatientService.Queries.GetStatistiques;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.PatientTests.Queries
{
    public class GetStatistiquesQueryHandlerTests
    {
        private readonly Mock<IPatientRepository> _repositoryMock;
        private readonly GetStatistiquesQueryHandler _handler;

        public GetStatistiquesQueryHandlerTests()
        {
            _repositoryMock = new Mock<IPatientRepository>();
            _handler = new GetStatistiquesQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenDateDebutIsAfterDateFin()
        {
            // Arrange
            var dateDebut = new DateTime(2024, 6, 10);
            var dateFin = new DateTime(2024, 6, 1);
            var query = new GetStatistiquesQuery(dateDebut, dateFin);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<ArgumentException>();
            ex.Which.Message.Should().Contain("La date de début doit être antérieure à la date de fin.");
        }

        [Fact]
        public async Task Handle_ShouldReturnStatistiques_WhenDatesAreValid()
        {
            // Arrange
            var dateDebut = new DateTime(2024, 6, 1);
            var dateFin = new DateTime(2024, 6, 10);
            var expectedStat = 42;
            var query = new GetStatistiquesQuery(dateDebut, dateFin);

            _repositoryMock.Setup(r => r.GetStatistiquesAsync(dateDebut, dateFin)).ReturnsAsync(expectedStat);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(expectedStat);
            _repositoryMock.Verify(r => r.GetStatistiquesAsync(dateDebut, dateFin), Times.Once);
        }
    }
}
