using Doctor.Application.DoctorServices.Queries.GetAllDoctors;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DoctorTests.Queries
{
    public class GetAllDoctorsQueryHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly GetAllDoctorsQueryHandler _handler;

        public GetAllDoctorsQueryHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new GetAllDoctorsQueryHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDtos_WhenMedecinsFound()
        {
            // Arrange
            var disponibilites = new List<Disponibilite>
            {
                new Disponibilite { Jour = DayOfWeek.Wednesday, HeureDebut = new TimeSpan(10,0,0), HeureFin = new TimeSpan(13,0,0) }
            };
            var medecins = new List<Medecin>
            {
                new Medecin
                {
                    Id = Guid.NewGuid(),
                    Prenom = "Luc",
                    Nom = "Bernard",
                    Specialite = "Généraliste",
                    CliniqueId = Guid.NewGuid(),
                    Email = "luc.bernard@email.com",
                    Telephone = "0612345678",
                    PhotoUrl = "http://photo.url",
                    Disponibilites = disponibilites
                }
            };
            var query = new GetAllDoctorsQuery();

            _medecinRepositoryMock
                .Setup(r => r.GetAllAsync())
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
            var query = new GetAllDoctorsQuery();

            _medecinRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Medecin>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
