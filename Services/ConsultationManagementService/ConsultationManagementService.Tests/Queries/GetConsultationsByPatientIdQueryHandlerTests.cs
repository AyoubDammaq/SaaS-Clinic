using ConsultationManagementService.Application.Queries.GetConsultationsByPatientId;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Queries
{
    public class GetConsultationsByPatientIdQueryHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly GetConsultationsByPatientIdQueryHandler _handler;

        public GetConsultationsByPatientIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _handler = new GetConsultationsByPatientIdQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_PatientIdIsEmpty()
        {
            // Arrange
            var query = new GetConsultationsByPatientIdQuery(Guid.Empty);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*patient*");
        }

        [Fact]
        public async Task Handle_Should_ReturnConsultations_When_PatientIdIsValid()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var query = new GetConsultationsByPatientIdQuery(patientId);
            var consultations = new List<Consultation>
                {
                    new Consultation { Id = Guid.NewGuid(), PatientId = patientId },
                    new Consultation { Id = Guid.NewGuid(), PatientId = patientId }
                };

            _repositoryMock.Setup(r => r.GetConsultationsByPatientIdAsync(patientId))
                .ReturnsAsync(consultations);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(consultations);
            _repositoryMock.Verify(r => r.GetConsultationsByPatientIdAsync(patientId), Times.Once);
        }
    }
}
