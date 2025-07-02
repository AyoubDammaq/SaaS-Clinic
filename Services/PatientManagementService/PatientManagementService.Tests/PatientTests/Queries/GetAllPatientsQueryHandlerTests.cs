using FluentAssertions;
using Moq;
using PatientManagementService.Application.PatientService.Queries.GetAllPatients;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.PatientTests.Queries
{
    public class GetAllPatientsQueryHandlerTests
    {
        private readonly Mock<IPatientRepository> _repositoryMock;
        private readonly GetAllPatientsQueryHandler _handler;

        public GetAllPatientsQueryHandlerTests()
        {
            _repositoryMock = new Mock<IPatientRepository>();
            _handler = new GetAllPatientsQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPatients_WhenRepositoryReturnsList()
        {
            // Arrange
            var patients = new List<Patient>
            {
                new Patient { Id = Guid.NewGuid(), Nom = "Dupont", Prenom = "Jean" },
                new Patient { Id = Guid.NewGuid(), Nom = "Martin", Prenom = "Marie" }
            };
            _repositoryMock.Setup(r => r.GetAllPatientsAsync()).ReturnsAsync(patients);

            // Act
            var result = await _handler.Handle(new GetAllPatientsQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(patients);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsNull()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllPatientsAsync()).ReturnsAsync((IEnumerable<Patient>?)null);

            // Act
            var result = await _handler.Handle(new GetAllPatientsQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldThrowApplicationException_WhenRepositoryThrows()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllPatientsAsync()).ThrowsAsync(new Exception("DB error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(new GetAllPatientsQuery(), CancellationToken.None);

            // Assert
            var ex = await act.Should().ThrowAsync<ApplicationException>();
            ex.Which.Message.Should().Contain("An error occurred while retrieving patients.");
        }
    }
}
