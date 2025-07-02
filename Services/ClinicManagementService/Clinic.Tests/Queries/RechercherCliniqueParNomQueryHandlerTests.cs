using Clinic.Application.Queries.RechercherCliniqueParNom;
using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Clinic.Tests.Queries
{
    public class RechercherCliniqueParNomQueryHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock = new();
        private readonly RechercherCliniqueParNomQueryHandler _handler;

        public RechercherCliniqueParNomQueryHandlerTests()
        {
            _handler = new RechercherCliniqueParNomQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidNom_ShouldReturnCliniques()
        {
            var cliniques = new List<Clinique?>
            {
                new Clinique { Id = Guid.NewGuid(), Nom = "Alpha" },
                null,
                new Clinique { Id = Guid.NewGuid(), Nom = "Beta" }
            };
            _repositoryMock.Setup(r => r.GetByNameAsync("A")).ReturnsAsync(cliniques);
            var query = new RechercherCliniqueParNomQuery("A");

            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

            result.Should().HaveCount(2);
            result.All(c => c != null).Should().BeTrue();
            _repositoryMock.Verify(r => r.GetByNameAsync("A"), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task Handle_WithEmptyNom_ShouldThrowArgumentException(string nom)
        {
            var query = new RechercherCliniqueParNomQuery(nom);

            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>();
            _repositoryMock.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
