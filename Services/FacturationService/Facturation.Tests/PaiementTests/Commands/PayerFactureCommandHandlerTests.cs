using Facturation.Application.DTOs;
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
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PayerFactureCommandHandler _handler;

        public PayerFactureCommandHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _paiementRepositoryMock = new Mock<IPaiementRepository>();
            _loggerMock = new Mock<ILogger<PayerFactureCommandHandler>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new PayerFactureCommandHandler(_paiementRepositoryMock.Object, _factureRepositoryMock.Object, _loggerMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFalse_When_FactureNotFound()
        {
            var request = new PayerFactureCommand(Guid.NewGuid(), new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = 100m });
            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(request.factureId)).ReturnsAsync((Facture?)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_Should_ReturnFalse_When_FactureAlreadyPayee()
        {
            var facture = new Facture { Id = Guid.NewGuid(), MontantTotal = 100m, MontantPaye = 100m, Status = FactureStatus.PAYEE };
            var request = new PayerFactureCommand(facture.Id, new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = 100m });
            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id)).ReturnsAsync(facture);

            var result = await _handler.Handle(request, CancellationToken.None);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Handle_Should_Throw_When_MontantIsInvalid(decimal montant)
        {
            var facture = new Facture { Id = Guid.NewGuid(), MontantTotal = 100m, MontantPaye = 0m, Status = FactureStatus.IMPAYEE };
            var request = new PayerFactureCommand(facture.Id, new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = montant });
            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id)).ReturnsAsync(facture);

            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Le montant payé est invalide*");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_MontantIsGreaterThanRestant()
        {
            var facture = new Facture { Id = Guid.NewGuid(), MontantTotal = 100m, MontantPaye = 50m, Status = FactureStatus.PARTIELLEMENT_PAYEE };
            var request = new PayerFactureCommand(facture.Id, new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = 60m });
            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id)).ReturnsAsync(facture);

            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Le montant payé est invalide*");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_CardDetailsMissing_ForCarteBancaire()
        {
            var facture = new Facture { Id = Guid.NewGuid(), MontantTotal = 100m, MontantPaye = 0m, Status = FactureStatus.IMPAYEE };
            var request = new PayerFactureCommand(facture.Id, new PaiementDto { MoyenPaiement = ModePaiement.CarteBancaire, Montant = 50m, CardDetails = null });
            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id)).ReturnsAsync(facture);

            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Détails de la carte requis*");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_CardDetailsInvalid_ForCarteBancaire()
        {
            var facture = new Facture { Id = Guid.NewGuid(), MontantTotal = 100m, MontantPaye = 0m, Status = FactureStatus.IMPAYEE };
            var cardDetails = new CardDetailsDto
            {
                CardholderName = "Jean Dupont",
                CardNumber = "1234567890123456", // Numéro non valide Luhn
                ExpiryDate = "12/99",
                Cvv = "123"
            };
            var request = new PayerFactureCommand(facture.Id, new PaiementDto { MoyenPaiement = ModePaiement.CarteBancaire, Montant = 50m, CardDetails = cardDetails });
            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id)).ReturnsAsync(facture);

            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Détails de la carte invalides*");
        }

        [Fact]
        public async Task Handle_Should_PayFactureCompletely_When_MontantEqualsRestant()
        {
            var facture = new Facture { Id = Guid.NewGuid(), MontantTotal = 100m, MontantPaye = 50m, Status = FactureStatus.PARTIELLEMENT_PAYEE };
            var request = new PayerFactureCommand(facture.Id, new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = 50m });
            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id)).ReturnsAsync(facture);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _handler.Handle(request, CancellationToken.None);

            result.Should().BeTrue();
            facture.MontantPaye.Should().Be(100m);
            facture.Status.Should().Be(FactureStatus.PAYEE);
            _paiementRepositoryMock.Verify(r => r.AddAsync(It.Is<Paiement>(p =>
                p.FactureId == facture.Id &&
                p.Mode == ModePaiement.Especes &&
                p.Montant == 50m
            )), Times.Once);
            _factureRepositoryMock.Verify(r => r.UpdateFactureAsync(facture), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_PartiallyPayFacture_When_MontantIsLessThanRestant()
        {
            var facture = new Facture { Id = Guid.NewGuid(), MontantTotal = 100m, MontantPaye = 20m, Status = FactureStatus.IMPAYEE };
            var request = new PayerFactureCommand(facture.Id, new PaiementDto { MoyenPaiement = ModePaiement.Virement, Montant = 30m });
            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id)).ReturnsAsync(facture);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _handler.Handle(request, CancellationToken.None);

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
        public async Task Handle_Should_RollbackTransaction_And_LogError_OnException()
        {
            var facture = new Facture { Id = Guid.NewGuid(), MontantTotal = 100m, MontantPaye = 0m, Status = FactureStatus.IMPAYEE };
            var request = new PayerFactureCommand(facture.Id, new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = 100m });
            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(facture.Id)).ReturnsAsync(facture);

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _paiementRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Paiement>())).ThrowsAsync(new Exception("Erreur DB"));
            _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>().WithMessage("Erreur DB");
            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
            _loggerMock.Verify(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>(), request.factureId), Times.Once);
        }
    }
}
