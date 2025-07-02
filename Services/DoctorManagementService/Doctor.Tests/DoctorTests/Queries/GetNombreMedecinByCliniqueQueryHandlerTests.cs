using Doctor.Application.DoctorServices.Queries.GetNombreMedecinByClinique;
using Doctor.Domain.Interfaces;
using Doctor.Domain.ValueObject;
using Moq;

namespace Doctor.Tests.DoctorTests.Queries
{
    public class GetNombreMedecinByCliniqueQueryHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly GetNombreMedecinByCliniqueQueryHandler _handler;

        public GetNombreMedecinByCliniqueQueryHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new GetNombreMedecinByCliniqueQueryHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDtos_WhenStatistiquesFound()
        {
            // Arrange
            var statistiques = new List<StatistiqueMedecin>
            {
                new StatistiqueMedecin { Cle = "Clinique A", Nombre = 5 },
                new StatistiqueMedecin { Cle = "Clinique B", Nombre = 3 }
            };
            var query = new GetNombreMedecinByCliniqueQuery();

            _medecinRepositoryMock
                .Setup(r => r.GetNombreMedecinByCliniqueAsync())
                .ReturnsAsync(statistiques);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("Clinique A", list[0].Cle);
            Assert.Equal(5, list[0].Nombre);
            Assert.Equal("Clinique B", list[1].Cle);
            Assert.Equal(3, list[1].Nombre);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoStatistiqueFound()
        {
            // Arrange
            var query = new GetNombreMedecinByCliniqueQuery();

            _medecinRepositoryMock
                .Setup(r => r.GetNombreMedecinByCliniqueAsync())
                .ReturnsAsync(new List<StatistiqueMedecin>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
