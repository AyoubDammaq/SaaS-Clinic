using Doctor.Application.AvailibilityServices.Queries.GetMedecinsDisponibles;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using Moq;

namespace Doctor.Tests.DisponibiliteTests.Queries
{
    public class GetMedecinsDisponiblesQueryHandlerTests
    {
        private readonly Mock<IDisponibiliteRepository> _disponibiliteRepositoryMock;
        private readonly GetMedecinsDisponiblesQueryHandler _handler;

        public GetMedecinsDisponiblesQueryHandlerTests()
        {
            _disponibiliteRepositoryMock = new Mock<IDisponibiliteRepository>();
            _handler = new GetMedecinsDisponiblesQueryHandler(_disponibiliteRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenDateIsDefault()
        {
            // Arrange
            var query = new GetMedecinsDisponiblesQuery(default, null, null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnMedecins_WhenRepositoryReturnsList()
        {
            // Arrange
            var date = new DateTime(2024, 6, 1);
            var heureDebut = new TimeSpan(8, 0, 0);
            var heureFin = new TimeSpan(12, 0, 0);
            var medecins = new List<Medecin>
            {
                new Medecin
                {
                    Id = Guid.NewGuid(),
                    Prenom = "Paul",
                    Nom = "Martin",
                    Specialite = "Cardiologie",
                    Email = "paul.martin@email.com",
                    Telephone = "0600000000",
                    PhotoUrl = "http://photo.url"
                }
            };
            var query = new GetMedecinsDisponiblesQuery(date, heureDebut, heureFin);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirMedecinsDisponiblesAsync(date, heureDebut, heureFin))
                .ReturnsAsync(medecins);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(medecins[0].Id, result[0].Id);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsNull()
        {
            // Arrange
            var date = new DateTime(2024, 6, 1);
            var query = new GetMedecinsDisponiblesQuery(date, null, null);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirMedecinsDisponiblesAsync(date, null, null))
                .ReturnsAsync((List<Medecin>)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsEmptyList()
        {
            // Arrange
            var date = new DateTime(2024, 6, 1);
            var query = new GetMedecinsDisponiblesQuery(date, null, null);

            _disponibiliteRepositoryMock
                .Setup(r => r.ObtenirMedecinsDisponiblesAsync(date, null, null))
                .ReturnsAsync(new List<Medecin>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
