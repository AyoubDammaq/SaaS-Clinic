using Facturation.Application.FactureService.Commands.DeleteFacture;
using Facturation.Domain.Entities;
using Facturation.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Facturation.Tests.FactureTests.Commands
{
    public class DeleteFactureCommandHandlerTests
    {
        private readonly Mock<IFactureRepository> _factureRepositoryMock;
        private readonly DeleteFactureCommandHandler _handler;

        public DeleteFactureCommandHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _handler = new DeleteFactureCommandHandler(_factureRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_DeleteFacture_When_IdIsValid()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var facture = new Facture { Id = factureId };
            var request = new DeleteFactureCommand(factureId);

            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(factureId))
                .ReturnsAsync(facture);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _factureRepositoryMock.Verify(r => r.GetFactureByIdAsync(factureId), Times.Once);
            _factureRepositoryMock.Verify(r => r.DeleteFactureAsync(factureId), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_IdIsEmpty()
        {
            // Arrange
            var request = new DeleteFactureCommand(Guid.Empty);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant de la facture ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_Should_ThrowKeyNotFoundException_When_FactureNotFound()
        {
            // Arrange
            var factureId = Guid.NewGuid();
            var request = new DeleteFactureCommand(factureId);

            _factureRepositoryMock.Setup(r => r.GetFactureByIdAsync(factureId))
                .ReturnsAsync((Facture?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"*{factureId}*");
        }
    }
}
