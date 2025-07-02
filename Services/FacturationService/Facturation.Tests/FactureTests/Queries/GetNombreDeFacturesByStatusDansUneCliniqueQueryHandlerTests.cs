using Facturation.Application.FactureService.Queries.GetNombreDeFacturesByStatusDansUneClinique;
using Facturation.Domain.Interfaces;
using Facturation.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Facturation.Tests.FactureTests.Queries
{
    public class GetNombreDeFacturesByStatusDansUneCliniqueQueryHandlerTests
    {
        private readonly Mock<IFactureRepository> _factureRepositoryMock;
        private readonly GetNombreDeFacturesByStatusDansUneCliniqueQueryHandler _handler;

        public GetNombreDeFacturesByStatusDansUneCliniqueQueryHandlerTests()
        {
            _factureRepositoryMock = new Mock<IFactureRepository>();
            _handler = new GetNombreDeFacturesByStatusDansUneCliniqueQueryHandler(_factureRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnMappedStats_When_StatsExist()
        {
            // Arrange
            var cliniqueId = Guid.NewGuid();
            var stats = new List<FactureStats>
                {
                    new FactureStats { Cle = "PAYEE", Nombre = 7 },
                    new FactureStats { Cle = "IMPAYEE", Nombre = 3 }
                };

            _factureRepositoryMock.Setup(r => r.GetNombreDeFacturesByStatusDansUneCliniqueAsync(cliniqueId))
                .ReturnsAsync(stats);

            var request = new GetNombreDeFacturesByStatusDansUneCliniqueQuery(cliniqueId);

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
            var cliniqueId = Guid.NewGuid();
            _factureRepositoryMock.Setup(r => r.GetNombreDeFacturesByStatusDansUneCliniqueAsync(cliniqueId))
                .ReturnsAsync(new List<FactureStats>());

            var request = new GetNombreDeFacturesByStatusDansUneCliniqueQuery(cliniqueId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentException_When_CliniqueIdIsEmpty()
        {
            // Arrange
            var request = new GetNombreDeFacturesByStatusDansUneCliniqueQuery(Guid.Empty);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*L'identifiant de la clinique ne peut pas être vide*");
        }
    }
}
