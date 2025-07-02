using Facturation.Application.EventHandlers;
using Facturation.Domain.Entities;
using Facturation.Domain.Events;
using Facturation.Domain.Interfaces.Messaging;
using Microsoft.Extensions.Logging;
using Moq;

namespace Facturation.Tests.EventHandlers
{
    public class FactureCreatedEventHandlerTests
    {
        private readonly Mock<IKafkaProducer> _producerMock;
        private readonly Mock<ILogger<FactureCreatedEventHandler>> _loggerMock;
        private readonly FactureCreatedEventHandler _handler;

        public FactureCreatedEventHandlerTests()
        {
            _producerMock = new Mock<IKafkaProducer>();
            _loggerMock = new Mock<ILogger<FactureCreatedEventHandler>>();
            _handler = new FactureCreatedEventHandler(_producerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_PublishEventAndLogInformation()
        {
            // Arrange
            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                MontantTotal = 123.45m
            };
            var notification = new FactureCreated(facture);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _producerMock.Verify(p => p.PublishAsync("facture-created", notification, It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains($"Nouvelle facture créée : {facture.Id}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
