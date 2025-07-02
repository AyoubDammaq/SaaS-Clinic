using Doctor.Application.DoctorServices.Commands.AddDoctor;
using Doctor.Application.DTOs;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DoctorTests.Commands
{
    public class AddDoctorCommandHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly AddDoctorCommandHandler _handler;

        public AddDoctorCommandHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new AddDoctorCommandHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenCreateMedecinDtoIsNull()
        {
            // Arrange
            var command = new AddDoctorCommand(null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldCallAddAsync_WithCorrectMedecin()
        {
            // Arrange
            var disponibilites = new List<Disponibilite>
            {
                new Disponibilite { Jour = DayOfWeek.Monday, HeureDebut = new TimeSpan(8,0,0), HeureFin = new TimeSpan(12,0,0) }
            };
            var dto = new CreateMedecinDto
            {
                Prenom = "Jean",
                Nom = "Dupont",
                Specialite = "Cardiologie",
                Email = "jean.dupont@email.com",
                Telephone = "0600000000",
                PhotoUrl = "http://photo.url",
                Disponibilites = disponibilites
            };
            var command = new AddDoctorCommand(dto);

            Medecin? medecinAjoute = null;
            _medecinRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Medecin>()))
                .Callback<Medecin>(m => medecinAjoute = m)
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _medecinRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Medecin>()), Times.Once);
            Assert.NotNull(medecinAjoute);
            Assert.Equal(dto.Prenom, medecinAjoute.Prenom);
            Assert.Equal(dto.Nom, medecinAjoute.Nom);
            Assert.Equal(dto.Specialite, medecinAjoute.Specialite);
            Assert.Equal(dto.Email, medecinAjoute.Email);
            Assert.Equal(dto.Telephone, medecinAjoute.Telephone);
            Assert.Equal(dto.PhotoUrl, medecinAjoute.PhotoUrl);
            Assert.Single(medecinAjoute.Disponibilites);
            Assert.Equal(DayOfWeek.Monday, medecinAjoute.Disponibilites.First().Jour);
        }

        [Fact]
        public async Task Handle_ShouldCallAddDoctorEvent()
        {
            // Arrange
            var dto = new CreateMedecinDto
            {
                Prenom = "Alice",
                Nom = "Martin",
                Specialite = "Dermatologie",
                Email = "alice.martin@email.com",
                Telephone = "0700000000",
                PhotoUrl = "http://photo.url",
                Disponibilites = new List<Disponibilite>()
            };
            var command = new AddDoctorCommand(dto);

            _medecinRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Medecin>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _medecinRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Medecin>()), Times.Once);
        }
    }
}
