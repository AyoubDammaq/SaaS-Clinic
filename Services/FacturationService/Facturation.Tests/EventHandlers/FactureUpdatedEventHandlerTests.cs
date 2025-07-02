using Facturation.Application.EventHandlers;
using Facturation.Domain.Entities;
using Facturation.Domain.Events;
using Facturation.Domain.Interfaces.Messaging;
using Microsoft.Extensions.Logging;
using Moq;

namespace Facturation.Tests.EventHandlers
{
    public class FactureUpdatedEventHandlerTests
    {
        private readonly Mock<IKafkaProducer> _producerMock;
        private readonly Mock<ILogger<FactureUpdatedEventHandler>> _loggerMock;
        private readonly FactureUpdatedEventHandler _handler;

        public FactureUpdatedEventHandlerTests()
        {
            _producerMock = new Mock<IKafkaProducer>();
            _loggerMock = new Mock<ILogger<FactureUpdatedEventHandler>>();
            _handler = new FactureUpdatedEventHandler(_producerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_PublishEventAndLogInformation()
        {
            // Arrange
            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                MontantTotal = 321.99m
            };
            var notification = new FactureUpdated(facture);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _producerMock.Verify(p => p.PublishAsync("facture-updated", notification, It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains($"Facture modifiée : {facture.Id}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
