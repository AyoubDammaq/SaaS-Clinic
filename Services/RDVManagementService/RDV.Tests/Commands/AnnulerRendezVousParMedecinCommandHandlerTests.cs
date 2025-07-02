using FluentAssertions;
using Moq;
using RDV.Application.Commands.AnnulerRendezVousParMedecin;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Commands
{
    public class AnnulerRendezVousParMedecinCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldAnnulerRendezVousParMedecin_WhenDataIsValid()
        {
            // Arrange
            var rendezVousId = Guid.NewGuid();
            var justification = "Absence du médecin";
            var rendezVous = new RendezVous(); // Utilisez l'objet réel, pas un mock
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByIdAsync(rendezVousId))
                .ReturnsAsync(rendezVous);

            var handler = new AnnulerRendezVousParMedecinCommandHandler(repoMock.Object);
            var command = new AnnulerRendezVousParMedecinCommand(rendezVousId, justification);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.AnnulerRendezVousParMedecin(rendezVousId, justification), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new AnnulerRendezVousParMedecinCommandHandler(repoMock.Object);
            var command = new AnnulerRendezVousParMedecinCommand(Guid.Empty, "justification");

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant du rendez-vous ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenJustificationIsEmpty()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new AnnulerRendezVousParMedecinCommandHandler(repoMock.Object);
            var command = new AnnulerRendezVousParMedecinCommand(Guid.NewGuid(), "");

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*La justification ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRendezVousNotFound()
        {
            // Arrange
            var rendezVousId = Guid.NewGuid();
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.GetRendezVousByIdAsync(rendezVousId))
                .ReturnsAsync((RendezVous)null);

            var handler = new AnnulerRendezVousParMedecinCommandHandler(repoMock.Object);
            var command = new AnnulerRendezVousParMedecinCommand(rendezVousId, "justification");

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}
