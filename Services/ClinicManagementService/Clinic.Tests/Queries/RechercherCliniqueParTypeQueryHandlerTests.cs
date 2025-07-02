using Clinic.Application.Queries.RechercherCliniqueParType;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Clinic.Tests.Queries
{
    public class RechercherCliniqueParTypeQueryHandlerTests
    {
        private readonly Mock<ICliniqueRepository> _repositoryMock = new();
        private readonly RechercherCliniqueParTypeQueryHandler _handler;

        public RechercherCliniqueParTypeQueryHandlerTests()
        {
            _handler = new RechercherCliniqueParTypeQueryHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithType_ShouldReturnCliniques()
        {
            var cliniques = new List<Clinique?>
            {
                new Clinique { Id = Guid.NewGuid(), TypeClinique = TypeClinique.Publique },
                null,
                new Clinique { Id = Guid.NewGuid(), TypeClinique = TypeClinique.Publique }
            };
            _repositoryMock.Setup(r => r.GetByTypeAsync(TypeClinique.Publique)).ReturnsAsync(cliniques);
            var query = new RechercherCliniqueParTypeQuery(TypeClinique.Publique);

            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

            result.Should().HaveCount(2);
            result.All(c => c != null).Should().BeTrue();
            _repositoryMock.Verify(r => r.GetByTypeAsync(TypeClinique.Publique), Times.Once);
        }
    }

}
