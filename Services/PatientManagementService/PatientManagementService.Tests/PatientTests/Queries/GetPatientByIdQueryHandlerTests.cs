using FluentAssertions;
using Moq;
using PatientManagementService.Application.PatientService.Queries.GetPatientById;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.PatientTests.Queries
{
    public class GetPatientByIdQueryHandlerTests
    {
        private readonly Mock<IPatientRepository> _repositoryMock;
        private readonly GetPatientByIdQueryHandler _handler;

        public GetPatientByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IPatientRepository>();
            _handler = new GetPatientByIdQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPatient_WhenPatientExists()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patient = new Patient
            {
                Id = patientId,
                Nom = "Durand",
                Prenom = "Alice"
            };
            _repositoryMock.Setup(r => r.GetPatientByIdAsync(patientId)).ReturnsAsync(patient);
            var query = new GetPatientByIdQuery(patientId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(patient);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenPatientDoesNotExist()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetPatientByIdAsync(patientId)).ReturnsAsync((Patient?)null);
            var query = new GetPatientByIdQuery(patientId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
