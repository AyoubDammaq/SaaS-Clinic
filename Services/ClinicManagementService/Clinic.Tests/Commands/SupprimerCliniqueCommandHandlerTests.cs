using Clinic.Application.Commands.SupprimerClinique;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Clinic.Tests.Commands
{
    public class SupprimerCliniqueCommandHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock;
        private readonly Mock<ILogger<SupprimerCliniqueCommandHandler>> _loggerMock;
        private readonly SupprimerCliniqueCommandHandler _handler;

        public SupprimerCliniqueCommandHandlerTests()
        {
            _repositoryMock = new Mock<ICliniqueRepository>();
            _loggerMock = new Mock<ILogger<SupprimerCliniqueCommandHandler>>();
            _handler = new SupprimerCliniqueCommandHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidId_ShouldDeleteCliniqueAndReturnTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var clinique = new Clinique
            {
                Id = id,
                Nom = "Clinique à supprimer",
                Adresse = "Adresse",
                NumeroTelephone = "12345678",
                Email = "test@clinique.fr",
                TypeClinique = TypeClinique.Publique,
                Statut = StatutClinique.Active
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(clinique);

            var command = new SupprimerCliniqueCommand(id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyId_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new SupprimerCliniqueCommand(Guid.Empty);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithUnknownId_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Clinique?)null);

            var command = new SupprimerCliniqueCommand(id);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
