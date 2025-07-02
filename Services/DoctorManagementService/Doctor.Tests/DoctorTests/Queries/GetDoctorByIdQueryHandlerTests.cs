using Doctor.Application.DoctorServices.Queries.GetDoctorById;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DoctorTests.Queries
{
    public class GetDoctorByIdQueryHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly GetDoctorByIdQueryHandler _handler;

        public GetDoctorByIdQueryHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new GetDoctorByIdQueryHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var query = new GetDoctorByIdQuery(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnDto_WhenMedecinFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var disponibilites = new List<Disponibilite>
            {
                new Disponibilite { Jour = DayOfWeek.Thursday, HeureDebut = new TimeSpan(14,0,0), HeureFin = new TimeSpan(18,0,0) }
            };
            var medecin = new Medecin
            {
                Id = id,
                Prenom = "Claire",
                Nom = "Petit",
                Specialite = "Neurologie",
                CliniqueId = Guid.NewGuid(),
                Email = "claire.petit@email.com",
                Telephone = "0611223344",
                PhotoUrl = "http://photo.url",
                Disponibilites = disponibilites
            };
            var query = new GetDoctorByIdQuery(id);

            _medecinRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(medecin);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(medecin.Id, result.Id);
            Assert.Equal(medecin.Prenom, result.Prenom);
            Assert.Equal(medecin.Nom, result.Nom);
            Assert.Equal(medecin.Specialite, result.Specialite);
            Assert.Equal(medecin.CliniqueId, result.CliniqueId);
            Assert.Equal(medecin.Email, result.Email);
            Assert.Equal(medecin.Telephone, result.Telephone);
            Assert.Equal(medecin.PhotoUrl, result.PhotoUrl);
            Assert.Equal(disponibilites, result.Disponibilites);
        }

        [Fact]
        public async Task Handle_ShouldThrowNullReferenceException_WhenMedecinNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetDoctorByIdQuery(id);

            _medecinRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Medecin)null);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }
    }
}
