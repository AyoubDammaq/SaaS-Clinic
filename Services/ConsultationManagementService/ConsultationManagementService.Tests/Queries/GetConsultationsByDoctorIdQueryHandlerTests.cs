using ConsultationManagementService.Application.Queries.GetConsultationsByDoctorId;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using FluentAssertions;
using Moq;

namespace ConsultationManagementService.Tests.Queries
{
    public class GetConsultationsByDoctorIdQueryHandlerTests
    {
        private readonly Mock<IConsultationRepository> _repositoryMock;
        private readonly GetConsultationsByDoctorIdQueryHandler _handler;

        public GetConsultationsByDoctorIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IConsultationRepository>();
            _handler = new GetConsultationsByDoctorIdQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_DoctorIdIsEmpty()
        {
            // Arrange
            var query = new GetConsultationsByDoctorIdQuery(Guid.Empty);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*médecin*");
        }

        [Fact]
        public async Task Handle_Should_ReturnConsultations_When_DoctorIdIsValid()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var query = new GetConsultationsByDoctorIdQuery(doctorId);
            var consultations = new List<Consultation>
                {
                    new Consultation { Id = Guid.NewGuid(), MedecinId = doctorId },
                    new Consultation { Id = Guid.NewGuid(), MedecinId = doctorId }
                };

            _repositoryMock.Setup(r => r.GetConsultationsByDoctorIdAsync(doctorId))
                .ReturnsAsync(consultations);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(consultations);
            _repositoryMock.Verify(r => r.GetConsultationsByDoctorIdAsync(doctorId), Times.Once);
        }
    }
}
