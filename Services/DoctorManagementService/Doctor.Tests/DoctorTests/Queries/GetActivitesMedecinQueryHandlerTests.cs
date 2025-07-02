using Doctor.Application.DoctorServices.Queries.GetActivitesMedecin;
using Doctor.Domain.Interfaces;
using Doctor.Domain.ValueObject;
using Moq;

namespace Doctor.Tests.DoctorTests.Queries
{
    public class GetActivitesMedecinQueryHandlerTests
    {
        private readonly Mock<IMedecinRepository> _medecinRepositoryMock;
        private readonly GetActivitesMedecinQueryHandler _handler;

        public GetActivitesMedecinQueryHandlerTests()
        {
            _medecinRepositoryMock = new Mock<IMedecinRepository>();
            _handler = new GetActivitesMedecinQueryHandler(_medecinRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenMedecinIdIsEmpty()
        {
            // Arrange
            var query = new GetActivitesMedecinQuery(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnDtos_WhenActivitesFound()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var activites = new List<ActiviteMedecin>
            {
                new ActiviteMedecin
                {
                    MedecinId = medecinId,
                    NomComplet = "Jean Dupont",
                    NombreConsultations = 10,
                    NombreRendezVous = 5
                }
            };
            var query = new GetActivitesMedecinQuery(medecinId);

            _medecinRepositoryMock
                .Setup(r => r.GetActivitesMedecinAsync(medecinId))
                .ReturnsAsync(activites);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var dto = Assert.Single(result);
            Assert.Equal(activites[0].MedecinId, dto.MedecinId);
            Assert.Equal(activites[0].NomComplet, dto.NomComplet);
            Assert.Equal(activites[0].NombreConsultations, dto.NombreConsultations);
            Assert.Equal(activites[0].NombreRendezVous, dto.NombreRendezVous);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoActiviteFound()
        {
            // Arrange
            var medecinId = Guid.NewGuid();
            var query = new GetActivitesMedecinQuery(medecinId);

            _medecinRepositoryMock
                .Setup(r => r.GetActivitesMedecinAsync(medecinId))
                .ReturnsAsync(new List<ActiviteMedecin>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
