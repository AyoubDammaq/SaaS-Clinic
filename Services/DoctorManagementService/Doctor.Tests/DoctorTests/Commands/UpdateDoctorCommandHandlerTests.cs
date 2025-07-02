using Doctor.Application.DoctorServices.Commands.UpdateDoctor;
using Doctor.Application.DTOs;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DoctorTests.Commands
{
    public class UpdateDoctorCommandHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly UpdateDoctorCommandHandler _handler;

        public UpdateDoctorCommandHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new UpdateDoctorCommandHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var command = new UpdateDoctorCommand(Guid.Empty, new MedecinDto());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenDtoIsNull()
        {
            // Arrange
            var command = new UpdateDoctorCommand(Guid.NewGuid(), null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenMedecinNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new MedecinDto
            {
                Prenom = "Paul",
                Nom = "Durand",
                Specialite = "Ophtalmologie"
            };
            var command = new UpdateDoctorCommand(id, dto);

            _medecinRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Medecin)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldUpdateMedecinAndCallUpdateAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            var disponibilites = new List<Disponibilite>
            {
                new Disponibilite { Jour = DayOfWeek.Friday, HeureDebut = new TimeSpan(9,0,0), HeureFin = new TimeSpan(12,0,0) }
            };
            var dto = new MedecinDto
            {
                Prenom = "Marie",
                Nom = "Lefevre",
                Specialite = "Pédiatrie",
                Email = "marie.lefevre@email.com",
                Telephone = "0612345678",
                PhotoUrl = "http://photo.url",
                Disponibilites = disponibilites
            };
            var command = new UpdateDoctorCommand(id, dto);

            var medecin = new Medecin
            {
                Id = id,
                Prenom = "Ancien",
                Nom = "Nom",
                Specialite = "Ancienne",
                Email = "ancien@email.com",
                Telephone = "0000000000",
                PhotoUrl = "http://ancien.url",
                Disponibilites = new List<Disponibilite>()
            };

            _medecinRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(medecin);

            _medecinRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Medecin>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _medecinRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Medecin>(m =>
                m.Prenom == dto.Prenom &&
                m.Nom == dto.Nom &&
                m.Specialite == dto.Specialite &&
                m.Email == dto.Email &&
                m.Telephone == dto.Telephone &&
                m.PhotoUrl == dto.PhotoUrl &&
                m.Disponibilites.SequenceEqual(disponibilites)
            )), Times.Once);
        }
    }
}
