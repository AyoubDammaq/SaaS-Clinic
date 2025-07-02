using AutoMapper;
using Clinic.Application.DTOs;
using Clinic.Application.Queries.GetNombreNouvellesCliniquesParMois;
using Clinic.Domain.Interfaces;
using Clinic.Domain.ValueObject;
using FluentAssertions;
using Moq;

namespace Clinic.Tests.Queries
{
    public class GetNombreNouvellesCliniquesParMoisQueryHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetNombreNouvellesCliniquesParMoisQueryHandler _handler;

        public GetNombreNouvellesCliniquesParMoisQueryHandlerTests()
        {
            _repositoryMock = new Mock<ICliniqueRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetNombreNouvellesCliniquesParMoisQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnStatistiqueDTOList_WhenStatistiquesExist()
        {
            // Arrange
            var statistiques = new List<Statistique>
                {
                    new Statistique { Cle = "Janvier 2024", Nombre = 2 },
                    new Statistique { Cle = "Février 2024", Nombre = 3 }
                };
            var statistiquesDto = new List<StatistiqueDTO>
                {
                    new StatistiqueDTO { Cle = "Janvier 2024", Nombre = 2 },
                    new StatistiqueDTO { Cle = "Février 2024", Nombre = 3 }
                };

            _repositoryMock.Setup(r => r.GetNombreNouvellesCliniquesParMoisAsync()).ReturnsAsync(statistiques);
            _mapperMock.Setup(m => m.Map<IEnumerable<StatistiqueDTO>>(statistiques)).Returns(statistiquesDto);

            var query = new GetNombreNouvellesCliniquesParMoisQuery();

            // Act
            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Cle.Should().Be("Janvier 2024");
            result[0].Nombre.Should().Be(2);
            result[1].Cle.Should().Be("Février 2024");
            result[1].Nombre.Should().Be(3);
            _repositoryMock.Verify(r => r.GetNombreNouvellesCliniquesParMoisAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenStatistiquesIsNull()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetNombreNouvellesCliniquesParMoisAsync()).ReturnsAsync((IEnumerable<Statistique>?)null);
            var query = new GetNombreNouvellesCliniquesParMoisQuery();

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Aucune statistique trouvée.");
            _repositoryMock.Verify(r => r.GetNombreNouvellesCliniquesParMoisAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidOperationException_WhenStatistiquesIsEmpty()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetNombreNouvellesCliniquesParMoisAsync()).ReturnsAsync(new List<Statistique>());
            var query = new GetNombreNouvellesCliniquesParMoisQuery();

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Aucune statistique trouvée.");
            _repositoryMock.Verify(r => r.GetNombreNouvellesCliniquesParMoisAsync(), Times.Once);
        }
    }
}
