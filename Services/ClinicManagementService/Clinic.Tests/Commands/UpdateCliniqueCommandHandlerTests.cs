using Clinic.Application.Commands.ModifierClinique;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Clinic.Tests.Commands
{
    public class UpdateCliniqueCommandHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock;
        private readonly Mock<ILogger<UpdateCliniqueCommandHandler>> _loggerMock;
        private readonly UpdateCliniqueCommandHandler _handler;

        public UpdateCliniqueCommandHandlerTests()
        {
            _repositoryMock = new Mock<ICliniqueRepository>();
            _loggerMock = new Mock<ILogger<UpdateCliniqueCommandHandler>>();
            _handler = new UpdateCliniqueCommandHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidData_ShouldUpdateCliniqueAndReturnIt()
        {
            // Arrange
            var id = Guid.NewGuid();
            var clinique = new Clinique
            {
                Id = id,
                Nom = "Clinique Modifiée",
                Adresse = "Adresse",
                NumeroTelephone = "12345678",
                Email = "test@clinique.fr",
                TypeClinique = TypeClinique.Publique,
                Statut = StatutClinique.Active
            };

            var command = new UpdateCliniqueCommand(id, clinique);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.Nom.Should().Be("Clinique Modifiée");
            _repositoryMock.Verify(r => r.UpdateAsync(clinique), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyId_ShouldThrowArgumentException()
        {
            // Arrange
            var clinique = new Clinique
            {
                Nom = "Clinique",
                Adresse = "Adresse",
                NumeroTelephone = "12345678",
                Email = "test@clinique.fr",
                TypeClinique = TypeClinique.Publique,
                Statut = StatutClinique.Active
            };
            var command = new UpdateCliniqueCommand(Guid.Empty, clinique);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Clinique>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithNullClinique_ShouldThrowArgumentNullException()
        {
            // Arrange
            var command = new UpdateCliniqueCommand(Guid.NewGuid(), (Clinique?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Clinique>()), Times.Never);
        }
    }
}
