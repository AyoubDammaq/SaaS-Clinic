using Facturation.Application.EventHandlers;
using Facturation.Domain.Entities;
using Facturation.Domain.Events;
using Facturation.Domain.Interfaces.Messaging;
using Microsoft.Extensions.Logging;
using Moq;

namespace Facturation.Tests.EventHandlers
{
    public class FacturePayedEventHandlerTests
    {
        private readonly Mock<IKafkaProducer> _producerMock;
        private readonly Mock<ILogger<FacturePayedEventHandler>> _loggerMock;
        private readonly FacturePayedEventHandler _handler;

        public FacturePayedEventHandlerTests()
        {
            _producerMock = new Mock<IKafkaProducer>();
            _loggerMock = new Mock<ILogger<FacturePayedEventHandler>>();
            _handler = new FacturePayedEventHandler(_producerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_PublishEventAndLogInformation()
        {
            // Arrange
            var paiement = new Paiement
            {
                Id = Guid.NewGuid(),
                FactureId = Guid.NewGuid()
            };
            var notification = new FacturePayed(paiement);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _producerMock.Verify(p => p.PublishAsync("facture-payed", notification, It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains($"Paiement effectué : {paiement.Id} pour la facture : {paiement.FactureId}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
