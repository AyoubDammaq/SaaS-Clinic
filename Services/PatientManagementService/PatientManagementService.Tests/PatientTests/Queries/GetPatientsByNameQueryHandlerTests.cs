using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PatientManagementService.Application.PatientService.Queries.GetPatientsByName;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.PatientTests.Queries
{
    public class GetPatientsByNameQueryHandlerTests
    {
        private readonly Mock<IPatientRepository> _repositoryMock;
        private readonly Mock<ILogger<GetPatientsByNameQueryHandler>> _loggerMock;
        private readonly GetPatientsByNameQueryHandler _handler;

        public GetPatientsByNameQueryHandlerTests()
        {
            _repositoryMock = new Mock<IPatientRepository>();
            _loggerMock = new Mock<ILogger<GetPatientsByNameQueryHandler>>();
            _handler = new GetPatientsByNameQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Theory]
        [InlineData("", "Durand")]
        [InlineData("Jean", "")]
        [InlineData(" ", "Durand")]
        [InlineData("Jean", " ")]
        public async Task Handle_ShouldReturnEmptyList_AndLogWarning_WhenNameOrLastnameIsNullOrWhiteSpace(string name, string lastname)
        {
            // Arrange
            var query = new GetPatientsByNameQuery(name, lastname);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Requête invalide : nom ou prénom manquant.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _repositoryMock.Verify(r => r.GetPatientsByNameAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_AndLogWarning_WhenNameAndLastnameAreBothNullOrWhiteSpace()
        {
            // Arrange
            var query = new GetPatientsByNameQuery("", "");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Requête invalide : nom ou prénom manquant.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _repositoryMock.Verify(r => r.GetPatientsByNameAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnPatients_WhenRepositoryReturnsList()
        {
            // Arrange
            var patients = new List<Patient>
                {
                    new Patient { Id = Guid.NewGuid(), Nom = "Durand", Prenom = "Jean" },
                    new Patient { Id = Guid.NewGuid(), Nom = "Durand", Prenom = "Marie" }
                };
            _repositoryMock.Setup(r => r.GetPatientsByNameAsync("Jean", "Durand")).ReturnsAsync(patients);
            var query = new GetPatientsByNameQuery("Jean", "Durand");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(patients);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsNull()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetPatientsByNameAsync("Jean", "Durand")).ReturnsAsync((IEnumerable<Patient>?)null);
            var query = new GetPatientsByNameQuery("Jean", "Durand");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
