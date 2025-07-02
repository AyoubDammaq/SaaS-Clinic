using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PatientManagementService.Application.PatientService.Commands.AddPatient;
using PatientManagementService.Application.PatientService.Commands.DeletePatient;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Tests.PatientTests.Commands
{
    public class DeletePatientCommandHandlerTests
    {
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<ILogger<DeletePatientCommandHandler>> _loggerMock;
        private readonly DeletePatientCommandHandler _handler;

        public DeletePatientCommandHandlerTests()
        {
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _loggerMock = new Mock<ILogger<DeletePatientCommandHandler>>();
            _handler = new DeletePatientCommandHandler(_patientRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_AndLogWarning_WhenPatientNotFound()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var command = new DeletePatientCommand(patientId);
            _patientRepositoryMock.Setup(r => r.GetPatientByIdAsync(patientId))
                .ReturnsAsync((Patient?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Patient introuvable pour suppression (ID : {patientId})")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            _patientRepositoryMock.Verify(r => r.DeletePatientAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallDelete_AndReturnTrue_WhenPatientExists()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patient = new Patient { Id = patientId };
            var command = new DeletePatientCommand(patientId);

            _patientRepositoryMock.Setup(r => r.GetPatientByIdAsync(patientId))
                .ReturnsAsync(patient);
            _patientRepositoryMock.Setup(r => r.DeletePatientAsync(patientId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _patientRepositoryMock.Verify(r => r.DeletePatientAsync(patientId), Times.Once);
        }
    }
}
