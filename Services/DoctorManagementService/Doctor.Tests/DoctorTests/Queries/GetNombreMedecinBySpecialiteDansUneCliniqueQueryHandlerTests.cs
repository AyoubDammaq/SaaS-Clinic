using Doctor.Application.DoctorServices.Queries.GetNombreMedecinBySpecialiteDansUneClinique;
using Doctor.Domain.Interfaces;
using Doctor.Domain.ValueObject;
using Moq;

namespace Doctor.Tests.DoctorTests.Queries
{
    public class GetNombreMedecinBySpecialiteDansUneCliniqueQueryHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly GetNombreMedecinBySpecialiteDansUneCliniqueQueryHandler _handler;

        public GetNombreMedecinBySpecialiteDansUneCliniqueQueryHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new GetNombreMedecinBySpecialiteDansUneCliniqueQueryHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenCliniqueIdIsEmpty()
        {
            // Arrange
            var query = new GetNombreMedecinBySpecialiteDansUneCliniqueQuery(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnDtos_WhenStatistiquesFound()
        {
            // Arrange
            var cliniqueId = Guid.NewGuid();
            var statistiques = new List<StatistiqueMedecin>
            {
                new StatistiqueMedecin { Cle = "Cardiologie", Nombre = 2 },
                new StatistiqueMedecin { Cle = "Pédiatrie", Nombre = 1 }
            };
            var query = new GetNombreMedecinBySpecialiteDansUneCliniqueQuery(cliniqueId);

            _medecinRepositoryMock
                .Setup(r => r.GetNombreMedecinBySpecialiteDansUneCliniqueAsync(cliniqueId))
                .ReturnsAsync(statistiques);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("Cardiologie", list[0].Cle);
            Assert.Equal(2, list[0].Nombre);
            Assert.Equal("Pédiatrie", list[1].Cle);
            Assert.Equal(1, list[1].Nombre);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoStatistiqueFound()
        {
            // Arrange
            var cliniqueId = Guid.NewGuid();
            var query = new GetNombreMedecinBySpecialiteDansUneCliniqueQuery(cliniqueId);

            _medecinRepositoryMock
                .Setup(r => r.GetNombreMedecinBySpecialiteDansUneCliniqueAsync(cliniqueId))
                .ReturnsAsync(new List<StatistiqueMedecin>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
