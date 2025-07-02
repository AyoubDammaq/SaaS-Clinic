using Facturation.Application.PaiementService.Commands.PayerFacture;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Facturation.Tests.PaiementTests.Commands
{
    public class PayerFactureCommandHandlerTests
    {
        private readonly Mock<IFactureRepository> _factureRepositoryMock;
        private readonly Mock<IPaiementRepository> _paiementRepositoryMock;
        private readonly Mock<ILogger<PayerFactureCommandHandler>> _loggerMock;
        private readonly PayerFactureCommandHandler _handler;

        public PayerFactureCommandHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _paiementRepositoryMock = new Mock<IPaiementRepository>();
            _loggerMock = new Mock<ILogger<PayerFactureCommandHandler>>();
            _handler = new PayerFactureCommandHandler(_paiementRepositoryMock.Object, _factureRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFalse_When_FactureNotFound()
        {
            // Arrange
            var request = new PayerFactureCommand(Guid.NewGuid(), ModePaiement.Especes, 100m);

            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(request.factureId))
                .ReturnsAsync((Facture?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_Should_ReturnFalse_When_FactureAlreadyPayee()
        {
            // Arrange
            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                MontantTotal = 100m,
                MontantPaye = 100m,
                Status = FactureStatus.PAYEE
            };
            var request = new PayerFactureCommand(facture.Id, ModePaiement.Especes, 100m);

            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id))
                .ReturnsAsync(facture);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Handle_Should_Throw_When_MontantIsInvalid(decimal montant)
        {
            // Arrange
            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                MontantTotal = 100m,
                MontantPaye = 0m,
                Status = FactureStatus.IMPAYEE
            };
            var request = new PayerFactureCommand(facture.Id, ModePaiement.Especes, montant);

            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id))
                .ReturnsAsync(facture);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Le montant payé est invalide*");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_MontantIsGreaterThanRestant()
        {
            // Arrange
            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                MontantTotal = 100m,
                MontantPaye = 50m,
                Status = FactureStatus.PARTIELLEMENT_PAYEE
            };
            var request = new PayerFactureCommand(facture.Id, ModePaiement.Especes, 60m);

            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id))
                .ReturnsAsync(facture);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Le montant payé est invalide*");
        }

        [Fact]
        public async Task Handle_Should_PayFactureCompletely_When_MontantEqualsRestant()
        {
            // Arrange
            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                MontantTotal = 100m,
                MontantPaye = 50m,
                Status = FactureStatus.PARTIELLEMENT_PAYEE
            };
            var request = new PayerFactureCommand(facture.Id, ModePaiement.CarteBancaire, 50m);

            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id))
                .ReturnsAsync(facture);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            facture.MontantPaye.Should().Be(100m);
            facture.Status.Should().Be(FactureStatus.PAYEE);
            _paiementRepositoryMock.Verify(r => r.AddAsync(It.Is<Paiement>(p =>
                p.FactureId == facture.Id &&
                p.Mode == ModePaiement.CarteBancaire &&
                p.Montant == 50m
            )), Times.Once);
            _factureRepositoryMock.Verify(r => r.UpdateFactureAsync(facture), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_PartiallyPayFacture_When_MontantIsLessThanRestant()
        {
            // Arrange
            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                MontantTotal = 100m,
                MontantPaye = 20m,
                Status = FactureStatus.IMPAYEE
            };
            var request = new PayerFactureCommand(facture.Id, ModePaiement.Virement, 30m);

            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id))
                .ReturnsAsync(facture);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            facture.MontantPaye.Should().Be(50m);
            facture.Status.Should().Be(FactureStatus.PARTIELLEMENT_PAYEE);
            _paiementRepositoryMock.Verify(r => r.AddAsync(It.Is<Paiement>(p =>
                p.FactureId == facture.Id &&
                p.Mode == ModePaiement.Virement &&
                p.Montant == 30m
            )), Times.Once);
            _factureRepositoryMock.Verify(r => r.UpdateFactureAsync(facture), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_CallPayerFactureEvent_And_UpdateFactureEvent()
        {
            // Arrange
            var facture = new Mock<Facture>();
            facture.SetupAllProperties();
            facture.Object.Id = Guid.NewGuid();
            facture.Object.MontantTotal = 100m;
            facture.Object.MontantPaye = 0m;
            facture.Object.Status = FactureStatus.IMPAYEE;

            var request = new PayerFactureCommand(facture.Object.Id, ModePaiement.Mobile, 100m);

            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Object.Id))
                .ReturnsAsync(facture.Object);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _paiementRepositoryMock.Verify(r => r.AddAsync(It.Is<Paiement>(p =>
                p.FactureId == facture.Object.Id &&
                p.Mode == ModePaiement.Mobile &&
                p.Montant == 100m
            )), Times.Once);
        }
    }
}
