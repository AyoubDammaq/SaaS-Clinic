using Doctor.Application.DoctorServices.Queries.GetMedecinByClinique;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DoctorTests.Queries
{
    public class GetMedecinByCliniqueQueryHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly GetMedecinByCliniqueQueryHandler _handler;

        public GetMedecinByCliniqueQueryHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new GetMedecinByCliniqueQueryHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenCliniqueIdIsEmpty()
        {
            // Arrange
            var query = new GetMedecinByCliniqueQuery(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnDtos_WhenMedecinsFound()
        {
            // Arrange
            var cliniqueId = Guid.NewGuid();
            var disponibilites = new List<Disponibilite>
            {
                new Disponibilite { Jour = DayOfWeek.Saturday, HeureDebut = new TimeSpan(8,0,0), HeureFin = new TimeSpan(12,0,0) }
            };
            var medecins = new List<Medecin>
            {
                new Medecin
                {
                    Id = Guid.NewGuid(),
                    Prenom = "Marc",
                    Nom = "Lemoine",
                    Specialite = "Gynécologie",
                    CliniqueId = cliniqueId,
                    Email = "marc.lemoine@email.com",
                    Telephone = "0611223344",
                    PhotoUrl = "http://photo.url",
                    Disponibilites = disponibilites
                }
            };
            var query = new GetMedecinByCliniqueQuery(cliniqueId);

            _medecinRepositoryMock
                .Setup(r => r.GetMedecinByCliniqueIdAsync(cliniqueId))
                .ReturnsAsync(medecins);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var dto = Assert.Single(result);
            Assert.Equal(medecins[0].Id, dto.Id);
            Assert.Equal(medecins[0].Prenom, dto.Prenom);
            Assert.Equal(medecins[0].Nom, dto.Nom);
            Assert.Equal(medecins[0].Specialite, dto.Specialite);
            Assert.Equal(medecins[0].CliniqueId, dto.CliniqueId);
            Assert.Equal(medecins[0].Email, dto.Email);
            Assert.Equal(medecins[0].Telephone, dto.Telephone);
            Assert.Equal(medecins[0].PhotoUrl, dto.PhotoUrl);
            Assert.Equal(disponibilites, dto.Disponibilites);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoMedecinFound()
        {
            // Arrange
            var cliniqueId = Guid.NewGuid();
            var query = new GetMedecinByCliniqueQuery(cliniqueId);

            _medecinRepositoryMock
                .Setup(r => r.GetMedecinByCliniqueIdAsync(cliniqueId))
                .ReturnsAsync(new List<Medecin>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
