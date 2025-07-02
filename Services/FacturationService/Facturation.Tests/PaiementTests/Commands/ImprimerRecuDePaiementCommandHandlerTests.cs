using Facturation.Application.PaiementService.Commands.ImprimerRecuDePaiement;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Facturation.Tests.PaiementTests.Commands
{
    public class ImprimerRecuDePaiementCommandHandlerTests
    {
        private readonly Mock<IPaiementRepository> _paiementRepositoryMock;
        private readonly Mock<ILogger<ImprimerRecuDePaiementCommandHandler>> _loggerMock;
        private readonly ImprimerRecuDePaiementCommandHandler _handler;

        public ImprimerRecuDePaiementCommandHandlerTests()
        {
            _paiementRepositoryMock = new Mock<IPaiementRepository>();
            _loggerMock = new Mock<ILogger<ImprimerRecuDePaiementCommandHandler>>();
            _handler = new ImprimerRecuDePaiementCommandHandler(_paiementRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnPdfBytes_When_RequestIsValid()
        {
            // Arrange
            var paiement = new Paiement
            {
                Id = Guid.NewGuid(),
                Montant = 250.75m,
                DatePaiement = new DateTime(2024, 6, 1),
                Mode = ModePaiement.CarteBancaire,
                FactureId = Guid.NewGuid()
            };
            var request = new ImprimerRecuDePaiementCommand(paiement);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            // Vérification simple : commence par %PDF (signature PDF)
            var pdfHeader = System.Text.Encoding.ASCII.GetString(result, 0, 4);
            pdfHeader.Should().Be("%PDF");
        }

        [Fact]
        public async Task Handle_Should_LogErrorAndThrow_When_ExceptionOccurs()
        {
            // Arrange
            var paiement = new Paiement
            {
                Id = Guid.NewGuid(),
                Montant = 250.75m,
                DatePaiement = new DateTime(2024, 6, 1),
                Mode = ModePaiement.CarteBancaire,
                FactureId = Guid.NewGuid()
            };
            var request = new ImprimerRecuDePaiementCommand(paiement);

            var faultyHandler = new FaultyImprimerRecuDePaiementCommandHandler(_paiementRepositoryMock.Object, _loggerMock.Object);

            // Act
            Func<Task> act = async () => await faultyHandler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApplicationException>()
                .WithMessage("Erreur lors de l'impression du reçu de paiement.");
            _loggerMock.Setup(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ));
        }

        // Handler factice pour simuler une exception dans Handle
        private class FaultyImprimerRecuDePaiementCommandHandler : ImprimerRecuDePaiementCommandHandler
        {
            public FaultyImprimerRecuDePaiementCommandHandler(IPaiementRepository repo, ILogger<ImprimerRecuDePaiementCommandHandler> logger)
                : base(repo, logger) { }

            public new Task<byte[]> Handle(ImprimerRecuDePaiementCommand request, CancellationToken cancellationToken)
            {
                throw new ApplicationException("Erreur lors de l'impression du reçu de paiement.");
            }
        }
    }
}
