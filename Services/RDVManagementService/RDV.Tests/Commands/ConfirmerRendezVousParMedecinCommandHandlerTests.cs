using FluentAssertions;
using Moq;
using RDV.Application.Commands.ConfirmerRendezVousParMedecin;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Commands
{
    public class ConfirmerRendezVousParMedecinCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldConfirmerRendezVous_WhenIdIsValid()
        {
            // Arrange
            var rendezVousId = Guid.NewGuid();
            var rendezVous = new RendezVous(); // Utilisez l'objet réel, pas un mock
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByIdAsync(rendezVousId))
                .ReturnsAsync(rendezVous);

            var handler = new ConfirmerRendezVousParMedecinCommandHandler(repoMock.Object);
            var command = new ConfirmerRendezVousParMedecinCommand(rendezVousId);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            // Assert
            repoMock.Verify(r => r.ConfirmerRendezVousParMedecin(rendezVousId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new ConfirmerRendezVousParMedecinCommandHandler(repoMock.Object);
            var command = new ConfirmerRendezVousParMedecinCommand(Guid.Empty);

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

            var handler = new ConfirmerRendezVousParMedecinCommandHandler(repoMock.Object);
            var command = new ConfirmerRendezVousParMedecinCommand(rendezVousId);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}
