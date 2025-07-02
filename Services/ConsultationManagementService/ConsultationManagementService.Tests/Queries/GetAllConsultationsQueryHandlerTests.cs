using ConsultationManagementService.Application.Queries.GetAllConsultations;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Queries
{
    public class GetAllConsultationsQueryHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly GetAllConsultationsQueryHandler _handler;

        public GetAllConsultationsQueryHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _handler = new GetAllConsultationsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_CallRepository_With_CorrectParameters_AndReturnResult()
        {
            // Arrange
            int pageNumber = 2;
            int pageSize = 5;
            var query = new GetAllConsultationsQuery(pageNumber, pageSize);
            var consultations = new List<Consultation>
                {
                    new Consultation { Id = Guid.NewGuid() },
                    new Consultation { Id = Guid.NewGuid() }
                };

            _repositoryMock.Setup(r => r.GetAllConsultationsAsync(pageNumber, pageSize))
                .ReturnsAsync(consultations);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(consultations);
            _repositoryMock.Verify(r => r.GetAllConsultationsAsync(pageNumber, pageSize), Times.Once);
        }
    }
}
