using ConsultationManagementService.Application.Queries.GetNombreConsultations;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Queries
{
    public class GetNombreConsultationsQueryHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly GetNombreConsultationsQueryHandler _handler;

        public GetNombreConsultationsQueryHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _handler = new GetNombreConsultationsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnCount_FromRepository()
        {
            // Arrange
            var dateDebut = new DateTime(2024, 1, 1);
            var dateFin = new DateTime(2024, 12, 31);
            var query = new GetNombreConsultationsQuery(dateDebut, dateFin);
            _repositoryMock.Setup(r => r.CountConsultationsAsync(dateDebut, dateFin)).ReturnsAsync(42);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(42);
            _repositoryMock.Verify(r => r.CountConsultationsAsync(dateDebut, dateFin), Times.Once);
        }
    }
}
