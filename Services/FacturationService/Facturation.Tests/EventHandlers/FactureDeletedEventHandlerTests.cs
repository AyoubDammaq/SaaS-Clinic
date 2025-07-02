using Facturation.Application.EventHandlers;
using Facturation.Domain.Events;
using Facturation.Domain.Interfaces.Messaging;
using Microsoft.Extensions.Logging;
using Moq;

namespace Facturation.Tests.EventHandlers
{
    public class FactureDeletedEventHandlerTests
    {
        private readonly Mock<IKafkaProducer> _producerMock;
        private readonly Mock<ILogger<FactureDeletedEventHandler>> _loggerMock;
        private readonly FactureDeletedEventHandler _handler;

        public FactureDeletedEventHandlerTests()
        {
            _producerMock = new Mock<IKafkaProducer>();
            _loggerMock = new Mock<ILogger<FactureDeletedEventHandler>>();
            _handler = new FactureDeletedEventHandler(_producerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_PublishEventAndLogInformation()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var notification = new FactureDeleted(factureId);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _producerMock.Verify(p => p.PublishAsync("facture-deleted", notification, It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains($"Facture supprimée : {factureId}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
