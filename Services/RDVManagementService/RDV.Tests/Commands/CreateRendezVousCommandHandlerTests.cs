using AutoMapper;
using FluentAssertions;
using Moq;
using RDV.Application.Commands.CreateRendezVous;
using RDV.Application.DTOs;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Tests.Commands
{
    public class CreateRendezVousCommandHandlerTests
    {
        private readonly Mock<IRendezVousRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateRendezVousCommandHandler _handler;

        public CreateRendezVousCommandHandlerTests()
        {
            _repoMock = new Mock<IRendezVousRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new CreateRendezVousCommandHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateRendezVous_WhenRendezVousIsValid_AndNoDouble()
        {
            // Arrange
            var dto = new CreateRendezVousDto
            {
                MedecinId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                DateHeure = DateTime.Now,
                Commentaire = "Test"
            };

            var entity = new RendezVous
            {
                MedecinId = dto.MedecinId,
                PatientId = dto.PatientId,
                DateHeure = dto.DateHeure,
                Commentaire = dto.Commentaire
            };

            _repoMock.Setup(r => r.ExisteRendezVousPourMedecinEtDate(dto.MedecinId, dto.DateHeure))
                .ReturnsAsync(false);

            _mapperMock.Setup(m => m.Map<RendezVous>(dto))
                .Returns(entity);

            _repoMock.Setup(r => r.CreateRendezVousAsync(entity))
                .Returns(Task.CompletedTask);

            var command = new CreateRendezVousCommand(dto);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repoMock.Verify(r => r.ExisteRendezVousPourMedecinEtDate(dto.MedecinId, dto.DateHeure), Times.Once);
            _repoMock.Verify(r => r.CreateRendezVousAsync(entity), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRendezVousIsNull()
        {
            // Arrange
            var command = new CreateRendezVousCommand(null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("*Le rendez-vous ne peut pas être nul*");
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenDoubleBooking()
        {
            // Arrange
            var dto = new CreateRendezVousDto
            {
                MedecinId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                DateHeure = DateTime.Now,
                Commentaire = "Test"
            };

            _repoMock.Setup(r => r.ExisteRendezVousPourMedecinEtDate(dto.MedecinId, dto.DateHeure))
                .ReturnsAsync(true);

            var command = new CreateRendezVousCommand(dto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Un rendez-vous existe déjà à cette heure pour ce médecin.");
            _repoMock.Verify(r => r.ExisteRendezVousPourMedecinEtDate(dto.MedecinId, dto.DateHeure), Times.Once);
            _repoMock.Verify(r => r.CreateRendezVousAsync(It.IsAny<RendezVous>()), Times.Never);
        }
    }
}
