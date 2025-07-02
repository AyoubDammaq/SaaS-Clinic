using FluentAssertions;
using Moq;
using RDV.Application.Commands.CreateRendezVous;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Commands
{
    public class CreateRendezVousCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreateRendezVous_WhenRendezVousIsValid_AndNoDouble()
        {
            // Arrange
            var rendezVous = new RendezVous
            {
                MedecinId = Guid.NewGuid(),
                DateHeure = DateTime.Now
            };
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.ExisteRendezVousPourMedecinEtDate(rendezVous.MedecinId, rendezVous.DateHeure))
                .ReturnsAsync(false);
            repoMock.Setup(r => r.CreateRendezVousAsync(rendezVous))
                .Returns(Task.CompletedTask);

            var handler = new CreateRendezVousCommandHandler(repoMock.Object);
            var command = new CreateRendezVousCommand(rendezVous);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.ExisteRendezVousPourMedecinEtDate(rendezVous.MedecinId, rendezVous.DateHeure), Times.Once);
            repoMock.Verify(r => r.CreateRendezVousAsync(rendezVous), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRendezVousIsNull()
        {
            // Arrange
            var repoMock = new Mock<IRendezVousRepository>();
            var handler = new CreateRendezVousCommandHandler(repoMock.Object);
            var command = new CreateRendezVousCommand(null);

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
            var rendezVous = new RendezVous
            {
                MedecinId = Guid.NewGuid(),
                DateHeure = DateTime.Now
            };
            var repoMock = new Mock<IRendezVousRepository>();
            repoMock.Setup(r => r.ExisteRendezVousPourMedecinEtDate(rendezVous.MedecinId, rendezVous.DateHeure))
                .ReturnsAsync(true);

            var handler = new CreateRendezVousCommandHandler(repoMock.Object);
            var command = new CreateRendezVousCommand(rendezVous);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Un rendez-vous existe déjà à cette heure pour ce médecin.");
            repoMock.Verify(r => r.ExisteRendezVousPourMedecinEtDate(rendezVous.MedecinId, rendezVous.DateHeure), Times.Once);
            repoMock.Verify(r => r.CreateRendezVousAsync(It.IsAny<RendezVous>()), Times.Never);
        }
    }
}
