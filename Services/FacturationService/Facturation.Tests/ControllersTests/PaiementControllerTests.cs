using Facturation.API.Controllers;
using Facturation.Application.DTOs;
using Facturation.Application.PaiementService.Commands.ImprimerRecuDePaiement;
using Facturation.Application.PaiementService.Commands.PayerFacture;
using Facturation.Application.PaiementService.Queries.GetPaiementByFactureId;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Facturation.Tests.ControllersTests
{
    public class PaiementControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PaiementController _controller;

        public PaiementControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PaiementController(_mediatorMock.Object);
        }

        [Fact]
        public async Task PayerFacture_Should_ReturnBadRequest_When_FactureIdIsEmpty()
        {
            // Arrange
            var dto = new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = 10m };

            // Act
            var result = await _controller.PayerFacture(Guid.Empty, dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PayerFacture_Should_ReturnBadRequest_When_ModePaiementIsInvalid()
        {
            // Arrange
            var dto = new PaiementDto { MoyenPaiement = (ModePaiement)999, Montant = 10m };

            // Act
            var result = await _controller.PayerFacture(Guid.NewGuid(), dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PayerFacture_Should_ReturnBadRequest_When_MontantIsInvalid()
        {
            // Arrange
            var dto = new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = 0m };

            // Act
            var result = await _controller.PayerFacture(Guid.NewGuid(), dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PayerFacture_Should_ReturnBadRequest_When_PaiementEchoue()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var dto = new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = 10m };
            _mediatorMock.Setup(m => m.Send(It.IsAny<PayerFactureCommand>(), default)).ReturnsAsync(false);

            // Act
            var result = await _controller.PayerFacture(factureId, dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PayerFacture_Should_ReturnOk_When_Success()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var dto = new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = 10m };
            _mediatorMock.Setup(m => m.Send(It.IsAny<PayerFactureCommand>(), default)).ReturnsAsync(true);

            // Act
            var result = await _controller.PayerFacture(factureId, dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PayerFacture_Should_ReturnBadRequest_When_InvalidOperationException()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var dto = new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = 10m };
            _mediatorMock.Setup(m => m.Send(It.IsAny<PayerFactureCommand>(), default))
                .ThrowsAsync(new InvalidOperationException("erreur"));

            // Act
            var result = await _controller.PayerFacture(factureId, dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PayerFacture_Should_Return500_When_Exception()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var dto = new PaiementDto { MoyenPaiement = ModePaiement.Especes, Montant = 10m };
            _mediatorMock.Setup(m => m.Send(It.IsAny<PayerFactureCommand>(), default))
                .ThrowsAsync(new Exception("fail"));

            // Act
            var result = await _controller.PayerFacture(factureId, dto);

            // Assert
            var status = result as ObjectResult;
            status.Should().NotBeNull();
            status!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task ImprimerRecuDePaiement_Should_ReturnBadRequest_When_FactureIdIsEmpty()
        {
            // Act
            var result = await _controller.ImprimerRecuDePaiement(Guid.Empty);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ImprimerRecuDePaiement_Should_ReturnNotFound_When_PaiementNotFound()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<GetPaiementByFactureIdQuery>(q => q.factureId == factureId), default))
                .ReturnsAsync((Paiement?)null);

            // Act
            var result = await _controller.ImprimerRecuDePaiement(factureId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ImprimerRecuDePaiement_Should_ReturnFile_When_PaiementExists()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var paiement = new Paiement
            {
                Id = Guid.NewGuid(),
                FactureId = factureId,
                DatePaiement = new DateTime(2024, 6, 1),
                Mode = ModePaiement.CarteBancaire,
                Montant = 100m
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetPaiementByFactureIdQuery>(q => q.factureId == factureId), default))
                .ReturnsAsync(paiement);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ImprimerRecuDePaiementCommand>(), default))
                .ReturnsAsync(new byte[] { 1, 2, 3, 4 });

            // Act
            var result = await _controller.ImprimerRecuDePaiement(factureId);

            // Assert
            result.Should().BeOfType<FileContentResult>();
        }

        [Fact]
        public async Task ImprimerRecuDePaiement_Should_Return500_When_Exception()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var paiement = new Paiement
            {
                Id = Guid.NewGuid(),
                FactureId = factureId,
                DatePaiement = new DateTime(2024, 6, 1),
                Mode = ModePaiement.CarteBancaire,
                Montant = 100m
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetPaiementByFactureIdQuery>(q => q.factureId == factureId), default))
                .ReturnsAsync(paiement);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ImprimerRecuDePaiementCommand>(), default))
                .ThrowsAsync(new Exception("fail"));

            // Act
            var result = await _controller.ImprimerRecuDePaiement(factureId);

            // Assert
            var status = result as ObjectResult;
            status.Should().NotBeNull();
            status!.StatusCode.Should().Be(500);
        }
    }
}
