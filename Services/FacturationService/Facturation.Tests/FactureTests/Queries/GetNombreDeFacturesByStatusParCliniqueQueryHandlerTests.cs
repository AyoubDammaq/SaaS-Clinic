using Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusParClinique;
using Facturation.Domain.Interfaces;
using Facturation.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Facturation.Tests.FactureTests.Queries
{
    public class GetNombreDeFacturesByStatusParCliniqueQueryHandlerTests
    {
        private readonly Mock<IFactureRepository> _factureRepositoryMock;
        private readonly GetNombreDeFacturesByStatusParCliniqueQueryHandler _handler;

        public GetNombreDeFacturesByStatusParCliniqueQueryHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _handler = new GetNombreDeFacturesByStatusParCliniqueQueryHandler(_factureRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnMappedStats_When_StatsExist()
        {
            // Arrange
            var stats = new List<FactureStats>
                {
                    new FactureStats { Cle = "CliniqueA_PAYEE", Nombre = 8 },
                    new FactureStats { Cle = "CliniqueB_IMPAYEE", Nombre = 4 }
                };

            _factureRepositoryMock.Setup(r => r.GetNombreDeFacturesByStatusParCliniqueAsync())
                .ReturnsAsync(stats);

            var request = new GetNombreDeFacturesByStatusParCliniqueQuery();

            // Act
            var result = (await _handler.Handle(request, CancellationToken.None)).ToList();

            // Assert
            result.Should().HaveCount(2);
            result[0].Cle.Should().Be(stats[0].Cle);
            result[0].Nombre.Should().Be(stats[0].Nombre);
            result[1].Cle.Should().Be(stats[1].Cle);
            result[1].Nombre.Should().Be(stats[1].Nombre);
        }

        [Fact]
        public async Task Handle_Should_ReturnEmptyList_When_NoStatsExist()
        {
            // Arrange
            _factureRepositoryMock.Setup(r => r.GetNombreDeFacturesByStatusParCliniqueAsync())
                .ReturnsAsync(new List<FactureStats>());

            var request = new GetNombreDeFacturesByStatusParCliniqueQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
