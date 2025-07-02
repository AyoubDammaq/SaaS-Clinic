using FluentAssertions;
using Moq;
using RDV.Application.Commands.AnnulerRendezVous;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Commands
{
    public class AnnulerRendezVousCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldAnnulerRendezVous_WhenIdIsValid()
        {
            // Arrange
            var rendezVousId = Guid.NewGuid();
            var rendezVous = new RendezVous(); // Utilisez l'objet réel, pas un mock
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByIdAsync(rendezVousId))
                .ReturnsAsync(rendezVous);
            repoMock.Setup(r => r.AnnulerRendezVousAsync(rendezVousId))
                .ReturnsAsync(true);

            var handler = new AnnulerRendezVousCommandHandler(repoMock.Object);
            var command = new AnnulerRendezVousCommand(rendezVousId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            repoMock.Verify(r => r.AnnulerRendezVousAsync(rendezVousId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new AnnulerRendezVousCommandHandler(repoMock.Object);
            var command = new AnnulerRendezVousCommand(Guid.Empty);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant du rendez-vous ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRendezVousNotFound()
        {
            // Arrange
            var rendezVousId = Guid.NewGuid();
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByIdAsync(rendezVousId))
                .ReturnsAsync((RendezVous)null);

            var handler = new AnnulerRendezVousCommandHandler(repoMock.Object);
            var command = new AnnulerRendezVousCommand(rendezVousId);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}
