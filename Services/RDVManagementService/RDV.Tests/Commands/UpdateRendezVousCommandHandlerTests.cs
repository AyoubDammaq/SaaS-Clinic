using FluentAssertions;
using Moq;
using RDV.Application.Commands.UpdateRendezVous;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Commands
{
    public class UpdateRendezVousCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldUpdateRendezVous_WhenDataIsValid_AndNoDouble()
        {
            // Arrange
            var rendezVousId = Guid.NewGuid();
            var rendezVous = new RendezVous
            {
                MedecinId = Guid.NewGuid(),
                DateHeure = DateTime.Now
            };
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.ExisteRendezVousPourMedecinEtDate(rendezVous.MedecinId, rendezVous.DateHeure))
                .ReturnsAsync(false);
            repoMock.Setup(r => r.UpdateRendezVousAsync(rendezVousId, rendezVous))
                .Returns(Task.CompletedTask);

            var handler = new UpdateRendezVousCommandHandler(repoMock.Object);
            var command = new UpdateRendezVousCommand(rendezVousId, rendezVous);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.ExisteRendezVousPourMedecinEtDate(rendezVous.MedecinId, rendezVous.DateHeure), Times.Once);
            repoMock.Verify(r => r.UpdateRendezVousAsync(rendezVousId, rendezVous), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new UpdateRendezVousCommandHandler(repoMock.Object);
            var command = new UpdateRendezVousCommand(Guid.Empty, new RendezVous());

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant du rendez-vous ne peut pas être vide*");
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRendezVousIsNull()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new UpdateRendezVousCommandHandler(repoMock.Object);
            var command = new UpdateRendezVousCommand(Guid.NewGuid(), null);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("*Le rendez-vous ne peut pas être nul*");
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenDoubleBooking()
        {
            // Arrange
            var rendezVousId = Guid.NewGuid();
            var rendezVous = new RendezVous
            {
                MedecinId = Guid.NewGuid(),
                DateHeure = DateTime.Now
            };
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.ExisteRendezVousPourMedecinEtDate(rendezVous.MedecinId, rendezVous.DateHeure))
                .ReturnsAsync(true);

            var handler = new UpdateRendezVousCommandHandler(repoMock.Object);
            var command = new UpdateRendezVousCommand(rendezVousId, rendezVous);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Un rendez-vous existe déjà à cette heure pour ce médecin.");
            repoMock.Verify(r => r.ExisteRendezVousPourMedecinEtDate(rendezVous.MedecinId, rendezVous.DateHeure), Times.Once);
            repoMock.Verify(r => r.UpdateRendezVousAsync(It.IsAny<Guid>(), It.IsAny<RendezVous>()), Times.Never);
        }
    }
}
