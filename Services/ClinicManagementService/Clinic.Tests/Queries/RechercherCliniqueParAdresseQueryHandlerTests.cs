using Clinic.Application.Queries.RechercherCliniqueParAdresse;
using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Clinic.Tests.Queries
{
    public class RechercherCliniqueParAdresseQueryHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock = new();
        private readonly RechercherCliniqueParAdresseQueryHandler _handler;

        public RechercherCliniqueParAdresseQueryHandlerTests()
        {
            _handler = new RechercherCliniqueParAdresseQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidAdresse_ShouldReturnCliniques()
        {
            var cliniques = new List<Clinique?>
            {
                new Clinique { Id = Guid.NewGuid(), Adresse = "Rue 1" },
                null,
                new Clinique { Id = Guid.NewGuid(), Adresse = "Rue 2" }
            };
            _repositoryMock.Setup(r => r.GetByAddressAsync("Rue")).ReturnsAsync(cliniques);
            var query = new RechercherCliniqueParAdresseQuery("Rue");

            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

            result.Should().HaveCount(2);
            result.All(c => c != null).Should().BeTrue();
            _repositoryMock.Verify(r => r.GetByAddressAsync("Rue"), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task Handle_WithEmptyAdresse_ShouldThrowArgumentException(string adresse)
        {
            var query = new RechercherCliniqueParAdresseQuery(adresse);

            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>();
            _repositoryMock.Verify(r => r.GetByAddressAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
