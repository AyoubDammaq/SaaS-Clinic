using ConsultationManagementService.Application.EventHandlers;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Domain.Events;
using ConsultationManagementService.Domain.Interfaces.Messaging;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConsultationManagementService.Tests.EventHandlers
{
    public class MedicalDocumentUploadedEventHandlerTests
    {
        private readonly Mock<IKafkaProducer> _producerMock;
        private readonly Mock<ILogger<MedicalDocumentUploadedEventHandler>> _loggerMock;
        private readonly MedicalDocumentUploadedEventHandler _handler;

        public MedicalDocumentUploadedEventHandlerTests()
        {
            _producerMock = new Mock<IKafkaProducer>();
            _loggerMock = new Mock<ILogger<MedicalDocumentUploadedEventHandler>>();
            _handler = new MedicalDocumentUploadedEventHandler(_producerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Publish_Message_And_Log_Info()
        {
            // Arrange
            var document = new DocumentMedical
            {
                Id = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                Type = "Ordonnance",
                FichierURL = "http://test.fr/file.pdf",
                DateAjout = DateTime.UtcNow
            };
            var evt = new MedicalDocumentUploaded(document);
            var cancellationToken = CancellationToken.None;

            _producerMock
                .Setup(p => p.PublishAsync("medicalDocument-uploaded", evt, cancellationToken))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _handler.Handle(evt, cancellationToken);

            // Assert
            _producerMock.Verify(p => p.PublishAsync("medicalDocument-uploaded", evt, cancellationToken), Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"📄 Document médical : {document.Id} téléchargé pour la consultation : {document.ConsultationId}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
