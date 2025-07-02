using Doctor.Application.DoctorServices.Queries.FilterDoctorsByName;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DoctorTests.Queries
{
    public class FilterDoctorsByNameQueryHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly FilterDoctorsByNameQueryHandler _handler;

        public FilterDoctorsByNameQueryHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new FilterDoctorsByNameQueryHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenNameAndPrenomAreEmpty()
        {
            // Arrange
            var query = new FilterDoctorsByNameQuery("   ", " ");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnDtos_WhenMedecinsFound()
        {
            // Arrange
            var disponibilites = new List<Disponibilite>
            {
                new Disponibilite { Jour = DayOfWeek.Monday, HeureDebut = new TimeSpan(8,0,0), HeureFin = new TimeSpan(12,0,0) }
            };
            var medecins = new List<Medecin>
            {
                new Medecin
                {
                    Id = Guid.NewGuid(),
                    Prenom = "Paul",
                    Nom = "Martin",
                    Specialite = "Cardiologie",
                    CliniqueId = Guid.NewGuid(),
                    Email = "paul.martin@email.com",
                    Telephone = "0600000000",
                    PhotoUrl = "http://photo.url",
                    Disponibilites = disponibilites
                }
            };
            var query = new FilterDoctorsByNameQuery("Martin", "Paul");

            _medecinRepositoryMock
                .Setup(r => r.FilterByNameOrPrenomAsync("Martin", "Paul", 1, 10))
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
            var query = new FilterDoctorsByNameQuery("Martin", "Paul");

            _medecinRepositoryMock
                .Setup(r => r.FilterByNameOrPrenomAsync("Martin", "Paul", 1, 10))
                .ReturnsAsync(new List<Medecin>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
