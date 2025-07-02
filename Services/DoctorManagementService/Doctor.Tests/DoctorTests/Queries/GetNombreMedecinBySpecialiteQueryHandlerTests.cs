using Doctor.Application.DoctorServices.Queries.GetNombreMedecinBySpecialite;
using Doctor.Domain.Interfaces;
using Doctor.Domain.ValueObject;
using Moq;

namespace Doctor.Tests.DoctorTests.Queries
{
    public class GetNombreMedecinBySpecialiteQueryHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly GetNombreMedecinBySpecialiteQueryHandler _handler;

        public GetNombreMedecinBySpecialiteQueryHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new GetNombreMedecinBySpecialiteQueryHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDtos_WhenStatistiquesFound()
        {
            // Arrange
            var statistiques = new List<StatistiqueMedecin>
            {
                new StatistiqueMedecin { Cle = "Cardiologie", Nombre = 7 },
                new StatistiqueMedecin { Cle = "Dermatologie", Nombre = 4 }
            };
            var query = new GetNombreMedecinBySpecialiteQuery();

            _medecinRepositoryMock
                .Setup(r => r.GetNombreMedecinBySpecialiteAsync())
                .ReturnsAsync(statistiques);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("Cardiologie", list[0].Cle);
            Assert.Equal(7, list[0].Nombre);
            Assert.Equal("Dermatologie", list[1].Cle);
            Assert.Equal(4, list[1].Nombre);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoStatistiqueFound()
        {
            // Arrange
            var query = new GetNombreMedecinBySpecialiteQuery();

            _medecinRepositoryMock
                .Setup(r => r.GetNombreMedecinBySpecialiteAsync())
                .ReturnsAsync(new List<StatistiqueMedecin>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
