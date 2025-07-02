using ConsultationManagementService.Application.EventHandlers;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Domain.Events;
using ConsultationManagementService.Domain.Interfaces.Messaging;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConsultationManagementService.Tests.EventHandlers
{
    public class ConsultationDeletedEventHandlerTests
    {
        private readonly Mock<IKafkaProducer> _producerMock;
        private readonly Mock<ILogger<ConsultationDeletedEventHandler>> _loggerMock;
        private readonly ConsultationDeletedEventHandler _handler;

        public ConsultationDeletedEventHandlerTests()
        {
            _producerMock = new Mock<IKafkaProducer>();
            _loggerMock = new Mock<ILogger<ConsultationDeletedEventHandler>>();
            _handler = new ConsultationDeletedEventHandler(_producerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Publish_Message_And_Log_Info()
        {
            // Arrange
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedecinId = Guid.NewGuid(),
                DateConsultation = DateTime.UtcNow,
                Diagnostic = "diagnostic",
                Notes = "notes",
                Documents = new System.Collections.Generic.List<DocumentMedical>()
            };
            var evt = new ConsultationDeleted(consultation);
            var cancellationToken = CancellationToken.None;

            _producerMock
                .Setup(p => p.PublishAsync("consultation-deleted", evt, cancellationToken))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _handler.Handle(evt, cancellationToken);

            // Assert
            _producerMock.Verify(p => p.PublishAsync("consultation-deleted", evt, cancellationToken), Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"🗑️ Consultation supprimée : {consultation.Id}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
