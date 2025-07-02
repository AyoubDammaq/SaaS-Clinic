using Facturation.Application.PaiementService.Queries.GetPaiementByFactureId;
using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Facturation.Tests.PaiementTests.Queries
{
    public class GetPaiementByFactureIdQueryHandlerTests
    {
        private readonly Mock<IPaiementRepository> _paiementRepositoryMock;
        private readonly GetPaiementByFactureIdQueryHandler _handler;

        public GetPaiementByFactureIdQueryHandlerTests()
        {
            _paiementRepositoryMock = new Mock<IPaiementRepository>();
            _handler = new GetPaiementByFactureIdQueryHandler(_paiementRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnPaiement_When_PaiementExists()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var paiement = new Paiement
            {
                Id = Guid.NewGuid(),
                FactureId = factureId,
                Montant = 123.45m,
                DatePaiement = DateTime.UtcNow,
                Mode = ModePaiement.CarteBancaire
            };

            _paiementRepositoryMock.Setup(r => r.GetByFactureIdAsync(factureId))
                .ReturnsAsync(paiement);

            var request = new GetPaiementByFactureIdQuery(factureId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(paiement);
        }

        [Fact]
        public async Task Handle_Should_ReturnNull_When_PaiementDoesNotExist()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            _paiementRepositoryMock.Setup(r => r.GetByFactureIdAsync(factureId))
                .ReturnsAsync((Paiement?)null);

            var request = new GetPaiementByFactureIdQuery(factureId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
